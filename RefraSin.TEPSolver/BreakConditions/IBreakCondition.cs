using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.BreakConditions;

public interface IBreakCondition : ISolverRoutine
{
    bool IsMet(SolutionState solutionState);
}
