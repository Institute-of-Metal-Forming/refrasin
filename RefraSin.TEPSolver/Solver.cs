using Microsoft.Extensions.Logging;
using RefraSin.ProcessModel;
using RefraSin.Storage;

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
    /// Storage for solution data.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; set; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; }

    internal ISolverSession CreateSession(ISinteringProcess process) => new SolverSession(this, process);

    /// <summary>
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    public void Solve(ISinteringProcess process)
    {
        throw new NotImplementedException();
    }
}