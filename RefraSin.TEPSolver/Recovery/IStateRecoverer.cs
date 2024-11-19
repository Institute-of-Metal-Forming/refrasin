using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.Recovery;

/// <summary>
/// Interface for classes meant to recover an invalid or unstable solution state to be able to proceed solution.
/// </summary>
public interface IStateRecoverer : ISolverRoutine
{
    SolutionState RecoverState(ISolverSession solverSession, SolutionState invalidState);
}
