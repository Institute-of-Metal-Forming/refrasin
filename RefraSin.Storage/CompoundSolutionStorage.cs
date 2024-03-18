using RefraSin.ProcessModel;

namespace RefraSin.Storage;

/// <summary>
/// A solution storage combining multiple other storages to use simultaneously.
/// </summary>
public class CompoundSolutionStorage : ISolutionStorage
{
    public CompoundSolutionStorage(IEnumerable<ISolutionStorage> storages)
    {
        Storages = storages.ToList();
    }

    /// <summary>
    /// List of storages to use simultaneously.
    /// </summary>
    public IList<ISolutionStorage> Storages;

    /// <inheritdoc />
    public void StoreState(ISystemState state)
    {
        foreach (var storage in Storages)
        {
            storage.StoreState(state);
        }
    }

    /// <inheritdoc />
    public void StoreStep(ISystemChange step)
    {
        foreach (var storage in Storages)
        {
            storage.StoreStep(step);
        }
    }
}