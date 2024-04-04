using RefraSin.ProcessModel;

namespace RefraSin.Storage;

public class InMemorySolutionStorage : ISolutionStorage
{
    /// <summary>
    /// List of all stored solution states.
    /// </summary>
    public IReadOnlyList<ISystemState> States => _states;

    private readonly List<ISystemState> _states = new();

    /// <summary>
    /// List of all stored solution steps.
    /// </summary>
    public IReadOnlyList<ISystemStateTransition> Steps => _steps;

    private readonly List<ISystemStateTransition> _steps = new();

    /// <inheritdoc />
    public void StoreState(IProcessStep processStep, ISystemState state)
    {
        _states.Add(state);
    }

    /// <inheritdoc />
    public void StoreStateTransition(IProcessStep processStep, ISystemStateTransition stateTransition)
    {
        _steps.Add(stateTransition);
    }
}