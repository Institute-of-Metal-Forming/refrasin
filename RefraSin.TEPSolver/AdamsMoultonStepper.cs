using RefraSin.TEPSolver.Step;

namespace RefraSin.TEPSolver;

internal class AdamsMoultonStepper : IStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, LagrangianGradient lagrangianGradient, StepVector initialGuess)
    {
        var step = lagrangianGradient.Solve(initialGuess);

        if (solverSession.LastStep is not null)
            return (step + solverSession.LastStep) / 2;
        return step;
    }
}