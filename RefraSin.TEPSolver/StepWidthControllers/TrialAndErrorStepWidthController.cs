using Microsoft.Extensions.Logging;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepWidthControllers;

public class TrialAndErrorStepWidthController(
    double initialTimeStepWidth = 1e-6,
    double minimalTimeStepWidth = double.NegativeInfinity,
    double maximalTimeStepWidth = double.PositiveInfinity,
    double increaseFactor = 1.5,
    double decreaseFactor = 0.5,
    int increaseDelay = 5
) : IStepWidthController
{
    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver)
    {
        solver.SessionInitialized += SolverOnSessionInitialized;
        solver.StepRejected += SolverOnStepRejected;
    }

    private void SolverOnSessionInitialized(
        object? sender,
        SinteringSolver.SessionInitializedEventArgs e
    )
    {
        _stepWidths[e.SolverSession.Id] = InitialTimeStepWidth;
        _stepsSinceLastIncrease[e.SolverSession.Id] = 0;
    }

    private void SolverOnStepRejected(object? sender, SinteringSolver.StepRejectedEventArgs e)
    {
        var currentStepWidth = _stepWidths[e.SolverSession.Id];

        if (currentStepWidth < MinimalTimeStepWidth)
            e.SolverSession.Logger.LogWarning(
                "Time step width can not be decreased further, since it fall below the allowed minimum."
            );
        else
        {
            _stepWidths[e.SolverSession.Id] *= DecreaseFactor;
            _stepsSinceLastIncrease[e.SolverSession.Id] = 0;
            e.SolverSession.Logger.LogInformation(
                "Time step width decreased to {Step}.",
                _stepWidths[e.SolverSession.Id]
            );
        }
    }

    /// <inheritdoc />
    public double GetStepWidth(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        if (_stepsSinceLastIncrease[solverSession.Id] < IncreaseDelay)
        {
            _stepsSinceLastIncrease[solverSession.Id]++;
        }
        else
        {
            if (_stepWidths[solverSession.Id] > MaximalTimeStepWidth)
                solverSession.Logger.LogWarning(
                    "Time step width can not be increased further, since it rose above the allowed maximum."
                );
            else
            {
                _stepWidths[solverSession.Id] *= IncreaseFactor;
                _stepsSinceLastIncrease[solverSession.Id] = 0;
            }
        }

        return _stepWidths[solverSession.Id];
    }

    private readonly Dictionary<Guid, double> _stepWidths = new();
    private readonly Dictionary<Guid, double> _stepsSinceLastIncrease = new();

    public double InitialTimeStepWidth { get; } = initialTimeStepWidth;
    public double IncreaseFactor { get; } = increaseFactor;
    public int IncreaseDelay { get; } = increaseDelay;
    public double DecreaseFactor { get; } = decreaseFactor;
    public double MinimalTimeStepWidth { get; } = minimalTimeStepWidth;
    public double MaximalTimeStepWidth { get; } = maximalTimeStepWidth;
}
