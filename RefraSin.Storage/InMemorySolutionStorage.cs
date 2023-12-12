namespace RefraSin.Storage;

public class InMemorySolutionStorage : ISolutionStorage
{
    /// <summary>
    /// List of all stored solution states.
    /// </summary>
    public IReadOnlyList<ISolutionState> States => _states;

    private readonly List<ISolutionState> _states = new();

    /// <summary>
    /// List of all stored solution steps.
    /// </summary>
    public IReadOnlyList<ISolutionStep> Steps => _steps;

    private readonly List<ISolutionStep> _steps = new();

    /// <inheritdoc />
    public void StoreState(ISolutionState state)
    {
        _states.Add(state);
    }

    /// <inheritdoc />
    public void StoreStep(ISolutionStep step)
    {
        _steps.Add(step);
    }
}