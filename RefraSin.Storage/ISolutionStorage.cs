using RefraSin.ProcessModel;

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
    void StoreState(ISystemState state);

    /// <summary>
    /// Store a solution step.
    /// </summary>
    /// <param name="step"></param>
    void StoreStep(ISystemChange step);

    /*
     * It is explicitly not planned to add some getter of states or steps here, since storage shall be considered as a black hole, fire and forget.
     * It is deferred to the actual implementation, if there shall be some getter logic and how it looks like.
     */
}