using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public interface ILagrangianRootFinder : ISolverRoutine
{
    public StepVector FindRoot(EquationSystem equationSystem, StepVector initialGuess);
}
