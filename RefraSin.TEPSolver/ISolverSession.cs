using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.Storage;
using Node = RefraSin.TEPSolver.ParticleModel.Node;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

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
    /// Registry for material and material interface data.
    /// </summary>
    public IReadOnlyMaterialRegistry MaterialRegistry { get; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILogger<Solver> Logger { get; }

    public LagrangianGradient LagrangianGradient { get; }

    public void RenewLagrangianGradient();

    public void IncreaseCurrentTime();

    public void IncreaseTimeStepWidth();

    public void DecreaseTimeStepWidth();

    public void StoreCurrentState();

    public void StoreStep(IEnumerable<IParticleTimeStep> particleTimeSteps);
}