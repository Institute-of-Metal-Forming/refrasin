using Microsoft.Extensions.Logging;
using MoreLinq;
using RefraSin.MaterialData;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using Node = RefraSin.TEPSolver.ParticleModel.Node;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

internal class SolverSession : ISolverSession
{
    private readonly IMaterialRegistry _materialRegistry;

    public SolverSession(Solver solver, ISinteringProcess process)
    {
        EndTime = process.EndTime;
        CurrentTime = process.StartTime;
        StartTime = CurrentTime;
        Temperature = process.Temperature;
        GasConstant = process.GasConstant;
        TimeStepWidth = solver.Options.InitialTimeStepWidth;
        TimeStepWidthOfLastStep = TimeStepWidth;
        Options = solver.Options;
        SolutionStorage = solver.SolutionStorage;
        _materialRegistry = new MaterialRegistry();

        foreach (var material in process.Materials)
            _materialRegistry.RegisterMaterial(material);

        foreach (var materialInterface in process.MaterialInterfaces)
            _materialRegistry.RegisterMaterialInterface(materialInterface);

        Logger = solver.LoggerFactory.CreateLogger<Solver>();

        var particles = process.ParticleSpecs.Select(ps => new Particle(null, ps, this)).ToArray();
        RootParticle = particles[0];
        Particles = particles.ToDictionary(p => p.Id);
        Nodes = Particles.Values.SelectMany(p => p.Surface).ToDictionary(n => n.Id);

        LagrangianGradient = new LagrangianGradient(this);
    }

    /// <inheritdoc />
    public double CurrentTime { get; set; }

    /// <inheritdoc />
    public double StartTime { get; set; }

    /// <inheritdoc />
    public double EndTime { get; set; }

    /// <inheritdoc />
    public double Temperature { get; }

    /// <inheritdoc />
    public double GasConstant { get; }

    /// <inheritdoc />
    public double TimeStepWidth { get; set; }

    /// <inheritdoc />
    public double? TimeStepWidthOfLastStep { get; set; }

    /// <inheritdoc />
    public Particle RootParticle { get; }

    /// <inheritdoc cref="ISolverSession.Particles"/>
    public Dictionary<Guid, Particle> Particles { get; set; }

    IReadOnlyDictionary<Guid, Particle> ISolverSession.Particles => Particles;

    /// <inheritdoc cref="ISolverSession.Nodes"/>
    public Dictionary<Guid, Node> Nodes { get; set; }

    IReadOnlyDictionary<Guid, Node> ISolverSession.Nodes => Nodes;

    /// <inheritdoc />
    public ISolverOptions Options { get; }

    /// <inheritdoc />
    public ISolutionStorage SolutionStorage { get; }

    /// <inheritdoc />
    public IReadOnlyMaterialRegistry MaterialRegistry => _materialRegistry;

    /// <inheritdoc />
    public ILogger<Solver> Logger { get; }

    /// <inheritdoc />
    public LagrangianGradient LagrangianGradient { get; private set; }

    /// <inheritdoc />
    public void RenewLagrangianGradient()
    {
        LagrangianGradient = new LagrangianGradient(this);
    }
}