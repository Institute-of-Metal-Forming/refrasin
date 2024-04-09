using Microsoft.Extensions.Logging;
using RefraSin.Enumerables;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.TEPSolver.TimeSteppers;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

internal class SolverSession : ISolverSession
{
    private readonly IMaterialRegistry _materialRegistry;

    private int _timeStepIndexWhereStepWidthWasLastModified = 0;

    private Action<ISystemState> _reportSystemState;
    private Action<ISystemStateTransition> _reportSystemStateTransition;

    public SolverSession(SinteringSolver sinteringSolver, ISystemState inputState, ISinteringStep step)
    {
        StartTime = inputState.Time;
        Duration = step.Duration;
        EndTime = StartTime + Duration;
        Temperature = step.Temperature;
        GasConstant = step.GasConstant;
        _reportSystemState = step.ReportSystemState;
        _reportSystemStateTransition = step.ReportSystemStateTransition;
        TimeStepWidth = sinteringSolver.Options.InitialTimeStepWidth;
        Options = sinteringSolver.Options;
        _materialRegistry = new MaterialRegistry();

        foreach (var material in inputState.Materials)
            _materialRegistry.RegisterMaterial(material);

        foreach (var materialInterface in inputState.MaterialInterfaces)
            _materialRegistry.RegisterMaterialInterface(materialInterface);

        Logger = sinteringSolver.LoggerFactory.CreateLogger<SinteringSolver>();

        StateMemory = new FixedStack<SolutionState>(Options.SolutionMemoryCount);
        Routines = sinteringSolver.Routines;

        var particles = inputState.Particles.Select(ps => new Particle(ps, this)).ToArray();
        CurrentState = new SolutionState(
            inputState.Id,
            StartTime,
            particles,
            inputState.Materials,
            inputState.MaterialInterfaces,
            Array.Empty<(Guid, Guid)>()
        );
        CurrentState = new SolutionState(
            inputState.Id,
            StartTime,
            particles,
            inputState.Materials,
            inputState.MaterialInterfaces,
            GetParticleContacts(particles)
        );
    }

    private static (Guid from, Guid to)[] GetParticleContacts(Particle[] particles)
    {
        var edges = particles.SelectMany(p => p.Nodes.OfType<NeckNode>())
            .Select(n => new UndirectedEdge<Particle>(n.Particle, n.ContactedNode.Particle));
        var graph = new UndirectedGraph<Particle>(particles, edges);
        var explorer = BreadthFirstExplorer<Particle>.Explore(graph, particles[0]);

        return explorer.TraversedEdges.Select(e => (e.From.Id, e.To.Id)).ToArray();
    }

    public double StartTime { get; }

    public double EndTime { get; }

    /// <inheritdoc />
    public double Duration { get; }

    /// <inheritdoc />
    public int TimeStepIndex { get; set; }

    /// <inheritdoc />
    public double Temperature { get; }

    /// <inheritdoc />
    public double GasConstant { get; }

    /// <inheritdoc />
    public double TimeStepWidth { get; private set; }

    /// <inheritdoc />
    public ISolverOptions Options { get; }

    public SolutionState CurrentState { get; set; }

    /// <inheritdoc />
    public IReadOnlyMaterialRegistry MaterialRegistry => _materialRegistry;

    /// <inheritdoc />
    public ILogger<SinteringSolver> Logger { get; }

    /// <inheritdoc />
    public ISolverRoutines Routines { get; }

    public StepVector? LastStep { get; set; }

    public FixedStack<SolutionState> StateMemory { get; }

    public void IncreaseTimeStepWidth()
    {
        TimeStepWidth *= Options.TimeStepAdaptationFactor;
        _timeStepIndexWhereStepWidthWasLastModified = TimeStepIndex;

        if (TimeStepWidth > Options.MaxTimeStepWidth)
        {
            TimeStepWidth = Options.MaxTimeStepWidth;
        }
    }

    public void MayIncreaseTimeStepWidth()
    {
        if (TimeStepIndex - _timeStepIndexWhereStepWidthWasLastModified > Options.TimeStepIncreaseDelay)
            IncreaseTimeStepWidth();
    }

    public void DecreaseTimeStepWidth()
    {
        TimeStepWidth /= Options.TimeStepAdaptationFactor;
        _timeStepIndexWhereStepWidthWasLastModified = TimeStepIndex;

        Logger.LogInformation("Time step width decreased to {TimeStepWidth}.", TimeStepWidth);

        if (TimeStepWidth < Options.MinTimeStepWidth)
        {
            throw new InvalidOperationException("Time step width was decreased below the allowed minimum.");
        }
    }

    public void ReportCurrentState()
    {
        _reportSystemState(CurrentState);
        StateMemory.Push(CurrentState);
    }

    public void ReportTransition(ISinteringStateStateTransition step)
    {
        _reportSystemStateTransition(step);
    }
}