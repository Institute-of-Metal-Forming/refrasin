using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

internal class AdamsMoultonTimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, StepVector initialGuess)
    {
        var step = solverSession.RootFinder.FindRoot(solverSession, solverSession.CurrentState, initialGuess);

        if (solverSession.LastStep is not null)
            return (step + solverSession.LastStep) / 2;
        return step;
    }
}