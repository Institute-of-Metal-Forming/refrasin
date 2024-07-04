using RefraSin.Enumerables;

namespace RefraSin.TEPSolver.Recovery;

public class StepBackStateRecoverer(int rememberedStateCount = 5) : IStateRecoverer
{
    /// <inheritdoc />
    public SolutionState RecoverState(ISolverSession solverSession, SolutionState invalidState)
    {
        try
        {
            return _stateMemory[solverSession.Id].Pop();
        }
        catch (InvalidOperationException e)
        {
            throw new RecoveryFailedException(this, "No earlier steps available.", e);
        }
    }

    private readonly Dictionary<Guid, FixedStack<SolutionState>> _stateMemory = new();

    public int RememberedStateCount { get; } = rememberedStateCount;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver)
    {
        solver.SessionInitialized += HandleSessionInitialized;
        solver.StepSuccessfullyCalculated += HandleStepSuccessfullyCalculated;
    }

    private void HandleSessionInitialized(object? sender, SinteringSolver.SessionInitializedEventArgs eventArgs)
    {
        _stateMemory.Add(eventArgs.SolverSession.Id, new FixedStack<SolutionState>(RememberedStateCount));
    }

    private void HandleStepSuccessfullyCalculated(object? sender, SinteringSolver.StepSuccessfullyCalculatedEventArgs eventArgs)
    {
        _stateMemory[eventArgs.SolverSession.Id].Push(eventArgs.NewState);
    }
}