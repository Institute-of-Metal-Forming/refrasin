using RefraSin.TEPSolver.Step;

namespace RefraSin.TEPSolver;

public interface IStepper
{
    public StepVector Step(ISolverSession solverSession, LagrangianGradient lagrangianGradient, StepVector initialGuess);
}