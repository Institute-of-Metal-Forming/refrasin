using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public interface ITimeStepper
{
    public StepVector Step(ISolverSession solverSession, StepVector initialGuess);
}