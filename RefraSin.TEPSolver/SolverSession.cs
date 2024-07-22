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

        MaterialInterfaces = step
            .MaterialInterfaces.Select(mi => new MaterialInterface(
                mi.From,
                mi.To,
                Norm.NormalizeInterfaceProperties(mi.Properties)
            ))
            .GroupBy(mi => mi.From)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<IMaterialInterface>)g.ToArray());

        Logger = sinteringSolver.LoggerFactory.CreateLogger<SinteringSolver>();
        Routines = sinteringSolver.Routines;

        CurrentState = new SolutionState(
            normalizedState.Id,
            StartTime,
            normalizedState,
            this
        );
        CurrentState.Sanitize();
    }

    public SolverSession(
        SolverSession parentSession,
        ISystemState inputState
    )
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
        MaterialInterfaces = parentSession.MaterialInterfaces;
        Logger = parentSession.Logger;
        Routines = parentSession.Routines;

        CurrentState = new SolutionState(
            inputState.Id,
            StartTime,
            inputState,
            this
        );
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

    public IReadOnlyDictionary<Guid, IReadOnlyList<IMaterialInterface>> MaterialInterfaces { get; }

    /// <inheritdoc />
    public ILogger<SinteringSolver> Logger { get; }

    /// <inheritdoc />
    public ISolverRoutines Routines { get; }

    /// <inheritdoc />
    public INorm Norm { get; }

    public void ReportCurrentState(StepVector? stepVector = null)
    {
        _reportSystemState(
            new SystemState(
                CurrentState.Id,
                CurrentState.Time,
                CurrentState.Particles.Select(p => new ParticleReturn(p, stepVector, Norm))
            )
        );
    }
}
