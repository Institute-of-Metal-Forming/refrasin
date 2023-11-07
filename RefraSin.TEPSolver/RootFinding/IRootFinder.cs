using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public interface IRootFinder
{
    public StepVector FindRoot(ISolverSession solverSession, StepVector initialGuess);
}