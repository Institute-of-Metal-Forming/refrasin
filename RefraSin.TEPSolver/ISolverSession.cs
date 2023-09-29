using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver;

/// <summary>
/// Interface for objects holding session data of a solution procedure.
/// </summary>
internal interface ISolverSession
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

    /// <summary>
    /// Step width used in the last time step.
    /// </summary>
    public double? TimeStepWidthOfLastStep { get; }

    /// <summary>
    /// Root particle of the particle tree.
    /// </summary>
    public Particle RootParticle { get; }

    /// <summary>
    /// Registry of all particles.
    /// </summary>
    public IReadOnlyDictionary<Guid, Particle> Particles { get; }

    /// <summary>
    /// Registry of all nodes of all particles.
    /// </summary>
    public IReadOnlyDictionary<Guid, Node> Nodes { get; }

    /// <summary>
    /// Options for the solver.
    /// </summary>
    public ISolverOptions Options { get; }

    /// <summary>
    /// Object for storing the calculated solution data. Store and forget from the perspective of the solver.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; }

    /// <summary>
    /// Registry for material and material interface data.
    /// </summary>
    public IReadOnlyMaterialRegistry MaterialRegistry { get; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILogger<Solver> Logger { get; }

    public LagrangianGradient LagrangianGradient { get; }

    public void RenewLagrangianGradient();
}