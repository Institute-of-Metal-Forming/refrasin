using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver;

public interface ISystemSolver
{
    public StepVector Solve(ISolverSession solverSession, StepVector initialGuess);
}