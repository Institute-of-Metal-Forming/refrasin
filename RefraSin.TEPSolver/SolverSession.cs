using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Enumerables;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

internal class SolverSession : ISolverSession
{
    private readonly Action<ISystemState> _reportSystemState;
    private readonly Action<ISystemStateTransition> _reportSystemStateTransition;

    public SolverSession(
        SinteringSolver sinteringSolver,
        ISystemState inputState,
        ISinteringStep step
    )
    {
        Id = Guid.NewGuid();
        Norm = sinteringSolver.Routines.Normalizer.GetNorm(inputState, step);

        StartTime = inputState.Time / Norm.Time;
        Duration = step.Duration / Norm.Time;
        EndTime = StartTime + Duration;
        Temperature = step.Temperature;
        GasConstant = step.GasConstant;
        _reportSystemState = step.ReportSystemState;
        _reportSystemStateTransition = step.ReportSystemStateTransition;

        Materials = step.Materials.ToDictionary(m => m.Id);
        MaterialInterfaces = step
            .MaterialInterfaces.GroupBy(mi => mi.From)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<IMaterialInterface>)g.ToArray());

        Logger = sinteringSolver.LoggerFactory.CreateLogger<SinteringSolver>();
        Routines = sinteringSolver.Routines;

        var particles = inputState.Particles.Select(ps => new Particle(ps, this)).ToArray();
        CurrentState = new SolutionState(
            inputState.Id,
            StartTime,
            particles,
            Array.Empty<(Guid, Guid, Guid)>()
        );
        CurrentState = new SolutionState(
            inputState.Id,
            StartTime,
            particles,
            GetParticleContacts(particles)
        );
    }

    private static (Guid id, Guid from, Guid to)[] GetParticleContacts(Particle[] particles)
    {
        var edges = particles
            .SelectMany(p => p.Nodes.OfType<NeckNode>())
            .Select(n => new UndirectedEdge<Particle>(n.Particle, n.ContactedNode.Particle));
        var graph = new UndirectedGraph<Particle>(particles, edges);
        var explorer = BreadthFirstExplorer<Particle>.Explore(graph, particles[0]);

        return explorer.TraversedEdges.Select(e => (e.Id, e.From.Id, e.To.Id)).ToArray();
    }

    public double StartTime { get; }

    public double EndTime { get; }

    /// <inheritdoc />
    public double Duration { get; }

    /// <inheritdoc />
    public double Temperature { get; }

    /// <inheritdoc />
    public double GasConstant { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    public SolutionState CurrentState { get; set; }

    /// <inheritdoc />
    public StepVector? LastStep { get; set; }

    public IReadOnlyDictionary<Guid, IMaterial> Materials { get; }

    public IReadOnlyDictionary<Guid, IReadOnlyList<IMaterialInterface>> MaterialInterfaces { get; }

    /// <inheritdoc />
    public ILogger<SinteringSolver> Logger { get; }

    /// <inheritdoc />
    public ISolverRoutines Routines { get; }

    /// <inheritdoc />
    public INorm Norm { get; }

    public void ReportCurrentState()
    {
        _reportSystemState(
            new SystemState(
                CurrentState.Id,
                CurrentState.Time * Norm.Time,
                CurrentState.Particles.Select(p =>
                {
                    var particleCenter =
                        new AbsolutePoint(
                            p.CenterCoordinates.X * Norm.Length,
                            p.CenterCoordinates.Y * Norm.Length
                        );
                    var particleSystem = new PolarCoordinateSystem(particleCenter, p.RotationAngle);
                    return new RefraSin.ParticleModel.Particle(
                        p.Id,
                        particleCenter,
                        p.RotationAngle,
                        p.MaterialId,
                        p.Nodes.Select(n => new Node(
                                n.Id,
                                p.Id,
                                new PolarPoint(n.Coordinates.Phi, n.Coordinates.R * Norm.Length, particleSystem),
                                n.Type
                            ))
                            .ToArray()
                    );
                })
            )
        );
    }

    public void ReportTransition(ISinteringStateStateTransition step)
    {
        _reportSystemStateTransition(step);
    }
}
