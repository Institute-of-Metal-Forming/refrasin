namespace RefraSin.Storage;

/// <summary>
/// Interface for classes providing storage for solution states and steps.
/// </summary>
public interface ISolutionStorage
{
    /// <summary>
    /// Store a solution state.
    /// </summary>
    /// <param name="state"></param>
    void StoreState(ISolutionState state);

    /// <summary>
    /// Store a solution step.
    /// </summary>
    /// <param name="step"></param>
    void StoreStep(ISolutionStep step);
}