using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

/// <summary>
/// Solver for performing time integration of sintering processes based on the thermodynamic extremal principle (TEP).
/// </summary>
public partial class Solver
{
    /// <summary>
    /// Numeric options to control solver behavior.
    /// </summary>
    public ISolverOptions Options { get; set; } = new SolverOptions();

    /// <summary>
    /// Registry for material and material interface data.
    /// </summary>
    public IMaterialRegistry MaterialRegistry { get; set; }

    /// <summary>
    /// Storage for solution data.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; set; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; }

    internal ISolverSession CreateSession(ISolutionState initialState, double endTime) => new SolverSession(this, initialState, endTime);

    /// <summary>
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    /// <param name="initialState"></param>
    /// <param name="endTime"></param>
    public void Solve(ISolutionState initialState, double endTime)
    {
        throw new NotImplementedException();
    }
}