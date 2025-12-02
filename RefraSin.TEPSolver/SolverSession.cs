using RefraSin.MaterialData;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.Vertex;
using Serilog;
using Log = Serilog.Log;

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
        PoreMaterial = step.PoreMaterial is null
            ? null
            : Norm.NormalizePoreMaterial(step.PoreMaterial);

        Routines = sinteringSolver.Routines;

        CurrentState = new SolutionState(
            normalizedState,
            Materials,
            normalizedConditions,
            PoreMaterial
        );
        CurrentState.Sanitize();
        Logger = Log.ForContext<SinteringSolver>();
        ParentSession = null;
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
        PoreMaterial = parentSession.PoreMaterial;
        Routines = parentSession.Routines;

        CurrentState = new SolutionState(inputState, Materials, this, PoreMaterial);
        CurrentState.Sanitize();
        Logger = parentSession.Logger;
        ParentSession = parentSession;
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

    public IReadOnlyList<IParticleMaterial> Materials { get; }

    public IPoreMaterial? PoreMaterial { get; set; }

    /// <inheritdoc />
    public ISolverRoutines Routines { get; }

    /// <inheritdoc />
    public INorm Norm { get; }

    /// <inheritdoc />
    public ISolverSession? ParentSession { get; }

    public void ReportCurrentState(StepVector? stepVector = null)
    {
        var particles = CurrentState
            .Particles.Select(p => new ParticleReturn(p, stepVector, Norm))
            .ToReadOnlyVertexCollection();

        if (CurrentState.Pores.Count == 0)
            _reportSystemState(
                new SystemState(CurrentState.Id, CurrentState.Time * Norm.Time, particles)
            );
        else
        {
            var nodes = particles.SelectMany(p => p.Nodes).ToReadOnlyVertexCollection();
            var pores = CurrentState.Pores.Select(p => new PoreReturn(
                p.Id,
                p.Nodes.Select(n => nodes[n.Id]),
                p.Porosity,
                p.HydrostaticStress,
                Norm
            ));
            _reportSystemState(
                new SystemStateWithPores(
                    CurrentState.Id,
                    CurrentState.Time * Norm.Time,
                    particles,
                    pores
                )
            );
        }
    }

    public ILogger Logger { get; }
}
