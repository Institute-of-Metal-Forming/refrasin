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
using static System.Math;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

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
        Norm = sinteringSolver.Routines.Normalizer.GetNorm(inputState, step);

        var normalizedState = Norm.NormalizeSystemState(inputState);

        StartTime = normalizedState.Time;
        Duration = step.Duration / Norm.Time;
        EndTime = StartTime + Duration;
        Temperature = step.Temperature / Norm.Temperature;
        GasConstant = step.GasConstant / Norm.Energy * Norm.Substance * Norm.Temperature;
        _reportSystemState = step.ReportSystemState;

        Materials = step.Materials.ToDictionary(m => m.Id, m => Norm.NormalizeMaterial(m));

        Logger = sinteringSolver.LoggerFactory.CreateLogger<SinteringSolver>();
        Routines = sinteringSolver.Routines;

        CurrentState = new SolutionState(normalizedState.Id, StartTime, normalizedState, Materials, Temperature, GasConstant);
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

        CurrentState = new SolutionState(inputState.Id, StartTime, inputState, Materials, Temperature, GasConstant);
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

    public IReadOnlyDictionary<Guid, IMaterial> Materials { get; }

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
