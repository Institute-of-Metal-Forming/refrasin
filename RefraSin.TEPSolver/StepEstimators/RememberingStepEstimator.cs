using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepEstimators;

public record RememberingStepEstimator(IStepEstimator Fallback) : IStepEstimator
{
    private readonly Dictionary<Guid, StepVector> _lastSteps = new();

    public StepVector EstimateStep(ISolverSession? solverSession, EquationSystem equationSystem) =>
        TryGetForCurrentSession(solverSession, equationSystem)
        ?? TryGetForParentSession(solverSession, equationSystem)
        ?? TryGetFromFallback(solverSession, equationSystem);

    private StepVector? TryGetForCurrentSession(
        ISolverSession? solverSession,
        EquationSystem equationSystem
    )
    {
        if (solverSession is not null && _lastSteps.TryGetValue(solverSession.Id, out var estimate))
            return estimate;
        return null;
    }

    private StepVector? TryGetForParentSession(
        ISolverSession? solverSession,
        EquationSystem equationSystem
    )
    {
        if (
            solverSession?.ParentSession is not null
            && _lastSteps.TryGetValue(solverSession.ParentSession.Id, out var parentEstimate)
        )
        {
            var map = new StepVectorMap(equationSystem);
            var estimate = new StepVector(new double[equationSystem.Size], map);

            foreach (var item in equationSystem.Items)
            {
                estimate.SetItemValue(
                    item,
                    parentEstimate.StepVectorMap.HasItem(item) ? parentEstimate.ItemValue(item) : 0
                );
            }

            return estimate;
        }

        return null;
    }

    private StepVector TryGetFromFallback(
        ISolverSession? solverSession,
        EquationSystem equationSystem
    ) => Fallback.EstimateStep(solverSession, equationSystem);

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
