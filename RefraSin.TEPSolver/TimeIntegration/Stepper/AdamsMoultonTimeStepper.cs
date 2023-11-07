using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver.TimeIntegration.Stepper;

internal class AdamsMoultonTimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, StepVector initialGuess)
    {
        var step = solverSession.SystemSolver.Solve(solverSession, initialGuess);

        if (solverSession.LastStep is not null)
            return (step + solverSession.LastStep) / 2;
        return step;
    }
}