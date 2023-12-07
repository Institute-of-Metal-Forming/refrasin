using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public interface IRootFinder
{
    public StepVector FindRoot(ISolverSession solverSession, SolutionState currentState, StepVector initialGuess);
}