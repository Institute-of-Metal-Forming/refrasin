using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver;

public interface IEquationSystemBuilder
{
    EquationSystem Build(SolutionState solutionState);
}
