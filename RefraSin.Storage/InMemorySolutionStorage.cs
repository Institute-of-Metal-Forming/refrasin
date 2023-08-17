namespace RefraSin.Storage;

public class InMemorySolutionStorage : ISolutionStorage
{
    /// <summary>
    /// List of all stored solution states.
    /// </summary>
    public IReadOnlyList<SolutionState> States => _states;

    private readonly List<SolutionState> _states = new();

    /// <summary>
    /// List of all stored solution steps.
    /// </summary>
    public IReadOnlyList<SolutionStep> Steps => _steps;

    private readonly List<SolutionStep> _steps = new();

    /// <inheritdoc />
    public void StoreState(ISolutionState state)
    {
        if (state is SolutionState s)
            _states.Add(s);
        else
            _states.Add(new SolutionState(state));
    }

    /// <inheritdoc />
    public void StoreStep(ISolutionStep step)
    {
        if (step is SolutionStep s)
            _steps.Add(s);
        else
            _steps.Add(new SolutionStep(step));
    }
}