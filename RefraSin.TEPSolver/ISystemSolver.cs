using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

public interface ISystemSolver
{
    public StepVector Solve(ISolverSession solverSession, StepVector initialGuess);
}