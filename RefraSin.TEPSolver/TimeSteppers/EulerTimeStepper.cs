using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public class EulerTimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, SolutionState baseState)
    {
        var equationSystem = solverSession.Routines.EquationSystemBuilder.Build(baseState);

        var estimate = _lastSteps.GetValueOrDefault(
            solverSession.Id,
            solverSession.Routines.StepEstimator.EstimateStep(equationSystem)
        );
        try
        {
            var step = solverSession.Routines.LagrangianRootFinder.FindRoot(
                equationSystem,
                estimate
            );
            return step;
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
