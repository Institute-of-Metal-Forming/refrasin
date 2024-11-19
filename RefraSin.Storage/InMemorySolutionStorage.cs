using RefraSin.ProcessModel;

namespace RefraSin.Storage;

public class InMemorySolutionStorage : ISolutionStorage
{
    /// <summary>
    /// List of all stored solution states.
    /// </summary>
    public IReadOnlyList<ISystemState> States => _states;

    private readonly List<ISystemState> _states = new();
    private readonly Dictionary<Guid, ISystemState> _statesDictionary = new();

    public ISystemState GetStateById(Guid id) => _statesDictionary[id];

    /// <inheritdoc />
    public void StoreState(IProcessStep processStep, ISystemState state)
    {
        _states.Add(state);
        _statesDictionary.Add(state.Id, state);
    }
}
