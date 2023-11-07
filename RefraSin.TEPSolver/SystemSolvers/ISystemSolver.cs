using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.SystemSolvers;

public interface ISystemSolver
{
    public StepVector Solve(ISolverSession solverSession, StepVector initialGuess);
}