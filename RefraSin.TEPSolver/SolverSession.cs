using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Enumerables;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Collections;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

internal class SolverSession : ISolverSession
{
    private readonly Action<ISystemState> _reportSystemState;

    public SolverSession(
        SinteringSolver sinteringSolver,
        ISystemState inputState,
        ISinteringStep step
    )
    {
        Id = Guid.NewGuid();
        Norm = sinteringSolver.Routines.Normalizer.GetNorm(inputState, step, step.Materials);

        var normalizedState = Norm.NormalizeSystemState(inputState);
        var normalizedConditions = Norm.NormalizeConditions(step);

        StartTime = normalizedState.Time;
        Duration = normalizedConditions.Duration;
        EndTime = StartTime + Duration;
        Temperature = normalizedConditions.Temperature;
        GasConstant = normalizedConditions.GasConstant;
        _reportSystemState = step.ReportSystemState;

        Materials = step.Materials.Select(m => Norm.NormalizeMaterial(m)).ToArray();

        Logger = sinteringSolver.LoggerFactory.CreateLogger<SinteringSolver>();
        Routines = sinteringSolver.Routines;

        CurrentState = new SolutionState(normalizedState, Materials, step);
        CurrentState.Sanitize();
    }

    public SolverSession(SolverSession parentSession, ISystemState inputState)
    {
        Id = Guid.NewGuid();
        Norm = parentSession.Norm;

        StartTime = inputState.Time;
        EndTime = parentSession.EndTime;
        Duration = EndTime - StartTime;
        Temperature = parentSession.Temperature;
        GasConstant = parentSession.GasConstant;
        _reportSystemState = parentSession._reportSystemState;

        Materials = parentSession.Materials;
        Logger = parentSession.Logger;
        Routines = parentSession.Routines;

        CurrentState = new SolutionState(inputState, Materials, this);
        CurrentState.Sanitize();
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

    public IReadOnlyList<IMaterial> Materials { get; }

    /// <inheritdoc />
    public ILogger<SinteringSolver> Logger { get; }

    /// <inheritdoc />
    public ISolverRoutines Routines { get; }

    /// <inheritdoc />
    public INorm Norm { get; }

    public void ReportCurrentState(StepVector? stepVector = null)
    {
        var particles = CurrentState
            .Particles.Select(p => new ParticleReturn(p, stepVector, Norm))
            .ToReadOnlyParticleCollection<ParticleReturn, NodeReturn>();
        var nodes = particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
        var particleContacts = CurrentState.ParticleContacts.Select(
            c => new ParticleContactEdge<ParticleReturn>(particles[c.From.Id], particles[c.To.Id])
        );
        var nodeContacts = CurrentState.NodeContacts.Select(c => new Edge<NodeReturn>(
            nodes[c.From.Id],
            nodes[c.To.Id],
            true
        ));
        _reportSystemState(
            new SystemState(
                CurrentState.Id,
                CurrentState.Time * Norm.Time,
                particles,
                particleContacts,
                nodeContacts
            )
        );
    }
}
