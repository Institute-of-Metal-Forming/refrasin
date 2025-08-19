using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.BreakConditions;

public interface IBreakCondition : ISolverRoutine
{
    bool IsMet(SolutionState solutionState);
}
