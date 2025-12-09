using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public class RungeKutta4TimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, SolutionState baseState)
    {
        var k1 = CalculateIntermediateStep(solverSession, baseState, null);
        var timeStep =
            solverSession.Routines.StepWidthControllers.Min(c =>
                c.GetStepWidth(solverSession, baseState, k1)
            ) ?? throw new InvalidOperationException("No step width could be computed.");

        var k2 = CalculateIntermediateStep(
            solverSession,
            baseState.ApplyTimeStep(k1, timeStep / 2),
            k1
        );
        var k3 = CalculateIntermediateStep(
            solverSession,
            baseState.ApplyTimeStep(k2, timeStep / 2),
            k2
        );
        var k4 = CalculateIntermediateStep(
            solverSession,
            baseState.ApplyTimeStep(k3, timeStep),
            k3
        );

        return (k1 + 2 * k2 + 2 * k3 + k4) / 6;
    }

    private StepVector CalculateIntermediateStep(
        ISolverSession solverSession,
        SolutionState baseState,
        StepVector? estimate
    )
    {
        var equationSystem = solverSession.Routines.EquationSystemBuilder.Build(baseState);
        estimate ??= solverSession.Routines.StepEstimator.EstimateStep(
            solverSession,
            equationSystem
        );
        try
        {
            return solverSession.Routines.LagrangianRootFinder.FindRoot(equationSystem, estimate);
        }
        catch (Exception e)
        {
            throw new StepFailedException(innerException: e);
        }
    }

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
