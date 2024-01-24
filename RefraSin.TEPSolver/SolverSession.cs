using Microsoft.Extensions.Logging;
using RefraSin.Enumerables;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
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
    private readonly ISolutionStorage _solutionStorage;

    private int _timeStepIndexWhereStepWidthWasLastModified = 0;

    public SolverSession(Solver solver, ISinteringProcess process)
    {
        StartTime = process.StartTime;
        EndTime = process.EndTime;
        Temperature = process.Temperature;
        GasConstant = process.GasConstant;
        TimeStepWidth = solver.Options.InitialTimeStepWidth;
        Options = solver.Options;
        _solutionStorage = solver.SolutionStorage;
        _materialRegistry = new MaterialRegistry();

        foreach (var material in process.Materials)
            _materialRegistry.RegisterMaterial(material);

        foreach (var materialInterface in process.MaterialInterfaces)
            _materialRegistry.RegisterMaterialInterface(materialInterface);

        Logger = solver.LoggerFactory.CreateLogger<Solver>();

        StateMemory = new FixedStack<SolutionState>(Options.SolutionMemoryCount);
        TimeStepper = solver.TimeStepper;
        StepValidators = solver.StepValidators.ToArray();
        RootFinder = solver.RootFinder;

        var particles = process.Particles.Select(ps => new Particle(ps, this)).ToArray();
        CurrentState = new SolutionState(
            StartTime,
            particles,
            Array.Empty<(Guid, Guid)>()
        );
        CurrentState = new SolutionState(
            StartTime,
            particles,
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

    /// <inheritdoc />
    public double StartTime { get; }

    /// <inheritdoc />
    public double EndTime { get; }

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
    public ILogger<Solver> Logger { get; }

    public StepVector? LastStep { get; set; }

    public ITimeStepper TimeStepper { get; }

    /// <inheritdoc />
    public IReadOnlyList<IStepValidator> StepValidators { get; }

    /// <inheritdoc />
    public IRootFinder RootFinder { get; }

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

    public void StoreCurrentState()
    {
        _solutionStorage.StoreState(CurrentState);
        StateMemory.Push(CurrentState);
    }

    public void StoreStep(ISolutionStep step)
    {
        _solutionStorage.StoreStep(step);
    }
}