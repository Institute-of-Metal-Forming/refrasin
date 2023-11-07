using Microsoft.Extensions.Logging;
using MoreLinq;
using RefraSin.Enumerables;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.Step;
using Node = RefraSin.TEPSolver.ParticleModel.Node;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

internal class SolverSession : ISolverSession
{
    private readonly IMaterialRegistry _materialRegistry;
    private readonly ISolutionStorage _solutionStorage;

    private int _timeStepIndexWhereStepWidthWasLastModified = 0;

    public SolverSession(Solver solver, ISinteringProcess process)
    {
        EndTime = process.EndTime;
        CurrentTime = process.StartTime;
        StartTime = CurrentTime;
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

        var particles = process.ParticleSpecs.Select(ps => new Particle(null, ps, this)).ToArray();
        Particles = particles.ToDictionary(p => p.Id);
        Nodes = Particles.Values.SelectMany(p => p.Surface).ToDictionary(n => n.Id);

        StateMemory = new FixedStack<ISolutionState>(Options.SolutionMemoryCount);
        Stepper = solver.Stepper;
    }

    /// <inheritdoc />
    public double CurrentTime { get; private set; }

    /// <inheritdoc />
    public double StartTime { get; }

    /// <inheritdoc />
    public double EndTime { get; }

    /// <inheritdoc />
    public int TimeStepIndex { get; private set; }

    /// <inheritdoc />
    public double Temperature { get; }

    /// <inheritdoc />
    public double GasConstant { get; }

    /// <inheritdoc />
    public double TimeStepWidth { get; private set; }

    /// <inheritdoc cref="ISolverSession.Particles"/>
    public Dictionary<Guid, Particle> Particles { get; private set; }

    IReadOnlyDictionary<Guid, Particle> ISolverSession.Particles => Particles;

    /// <inheritdoc cref="ISolverSession.Nodes"/>
    public Dictionary<Guid, Node> Nodes { get; private set; }

    IReadOnlyDictionary<Guid, Node> ISolverSession.Nodes => Nodes;

    /// <inheritdoc />
    public ISolverOptions Options { get; }

    /// <inheritdoc />
    public IReadOnlyMaterialRegistry MaterialRegistry => _materialRegistry;

    /// <inheritdoc />
    public ILogger<Solver> Logger { get; }


    public StepVector? LastStep { get; set; }

    public IStepper Stepper { get; }

    public FixedStack<ISolutionState> StateMemory { get; }

    public void IncreaseCurrentTime()
    {
        TimeStepIndex++;
        CurrentTime += TimeStepWidth;
    }

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
        var solutionState = new SolutionState(CurrentTime, Particles.Values);
        _solutionStorage.StoreState(solutionState);
        StateMemory.Push(solutionState);
    }

    public void StoreStep(IEnumerable<IParticleTimeStep> particleTimeSteps)
    {
        var nextTime = CurrentTime + TimeStepWidth;
        _solutionStorage.StoreStep(new SolutionStep(CurrentTime, nextTime, particleTimeSteps));
    }

    public void ResetTo(ISolutionState state)
    {
        CurrentTime = state.Time;

        var particleStates = state.ParticleStates.ToDictionary(n => n.Id);

        foreach (var particle in Particles.Values)
        {
            particle.ApplyState(particleStates[particle.Id]);
        }
    }
}