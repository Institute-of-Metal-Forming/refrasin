using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver.TimeIntegration.Stepper;

public interface ITimeStepper
{
    public StepVector Step(ISolverSession solverSession, StepVector initialGuess);
}