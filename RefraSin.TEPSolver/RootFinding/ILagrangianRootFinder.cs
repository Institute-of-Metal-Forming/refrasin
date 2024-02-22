using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public interface ILagrangianRootFinder
{
    public StepVector FindRoot(ISolverSession solverSession, SolutionState currentState, StepVector initialGuess);
}