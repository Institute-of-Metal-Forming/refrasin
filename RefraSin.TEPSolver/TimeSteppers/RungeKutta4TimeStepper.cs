using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public class RungeKutta4TimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, SolutionState baseState)
    {
        var k1 = CalculateIntermediateStep(solverSession, baseState, null);
        var timeStep = solverSession.Routines.StepWidthController.GetStepWidth(
            solverSession,
            baseState,
            k1
        );

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
        estimate ??= _lastSteps.GetValueOrDefault(
            solverSession.Id,
            solverSession.Routines.StepEstimator.EstimateStep(equationSystem)
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

    private readonly Dictionary<Guid, StepVector> _lastSteps = new();

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver)
    {
        solver.StepSuccessfullyCalculated += HandleStepSuccessfullyCalculated;
    }

    private void HandleStepSuccessfullyCalculated(
        object? sender,
        SinteringSolver.StepSuccessfullyCalculatedEventArgs e
    )
    {
        _lastSteps[e.SolverSession.Id] = e.StepVector;
    }
}
