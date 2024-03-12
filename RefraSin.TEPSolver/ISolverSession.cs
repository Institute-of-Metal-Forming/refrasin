using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.TEPSolver.TimeSteppers;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

/// <summary>
/// Interface for objects holding session data of a solution procedure.
/// </summary>
public interface ISolverSession : IProcessConditions
{
    /// <summary>
    /// Time where the solution started.
    /// </summary>
    public double StartTime { get; }

    /// <summary>
    /// Time where the solution should end.
    /// </summary>
    public double EndTime { get; }

    public int TimeStepIndex { get; }

    /// <summary>
    /// Current step width of time integration.
    /// </summary>
    public double TimeStepWidth { get; }

    public StepVector? LastStep { get; }

    /// <summary>
    /// Options for the solver.
    /// </summary>
    public ISolverOptions Options { get; }

    /// <summary>
    /// Registry for material and material interface data.
    /// </summary>
    public IReadOnlyMaterialRegistry MaterialRegistry { get; }

    public SolutionState CurrentState { get; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILogger<Solver> Logger { get; }

    /// <summary>
    /// COllection of solver routines to use.
    /// </summary>
    public ISolverRoutines Routines { get; }
}