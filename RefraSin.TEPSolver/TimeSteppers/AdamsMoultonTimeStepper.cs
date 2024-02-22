using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public class AdamsMoultonTimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, StepVector initialGuess)
    {
        var step = solverSession.Routines.LagrangianRootFinder.FindRoot(solverSession, solverSession.CurrentState, initialGuess);

        if (solverSession.LastStep is not null)
            return (step + solverSession.LastStep) / 2;
        return step;
    }
}