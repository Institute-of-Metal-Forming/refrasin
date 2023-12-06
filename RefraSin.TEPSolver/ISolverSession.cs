using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
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
public interface ISolverSession
{
    /// <summary>
    /// Time of the current step.
    /// </summary>
    public double CurrentTime { get; }

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
    /// Constant process temperature.
    /// </summary>
    public double Temperature { get; }

    /// <summary>
    /// Universal gas constant R.
    /// </summary>
    public double GasConstant { get; }

    /// <summary>
    /// Current step width of time integration.
    /// </summary>
    public double TimeStepWidth { get; }

    public StepVector? LastStep { get; }

    /// <summary>
    /// Registry of all particles.
    /// </summary>
    public IReadOnlyDictionary<Guid, Particle> Particles { get; }

    /// <summary>
    /// Registry of all nodes of all particles.
    /// </summary>
    public IReadOnlyDictionary<Guid, NodeBase> Nodes { get; }

    /// <summary>
    /// Options for the solver.
    /// </summary>
    public ISolverOptions Options { get; }

    /// <summary>
    /// Registry for material and material interface data.
    /// </summary>
    public IReadOnlyMaterialRegistry MaterialRegistry { get; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILogger<Solver> Logger { get; }

    public ITimeStepper TimeStepper { get; }

    public IReadOnlyList<IStepValidator> StepValidators { get; }

    public IRootFinder RootFinder { get; }
}