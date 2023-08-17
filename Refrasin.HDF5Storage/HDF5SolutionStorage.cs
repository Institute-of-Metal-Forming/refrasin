using HDF5CSharp;
using RefraSin.Storage;

namespace Refrasin.HDF5Storage;

public class HDF5SolutionStorage : ISolutionStorage, IDisposable
{
    public HDF5SolutionStorage(string filePath, string statesGroupName = "States", string stepsGroupName = "Steps")
    {
        FilePath = filePath;
        FileId = Hdf5.CreateFile(FilePath);

        StatesGroupId = Hdf5.CreateOrOpenGroup(FileId, statesGroupName);
        StepsGroupId = Hdf5.CreateOrOpenGroup(FileId, stepsGroupName);
    }

    public string FilePath { get; }

    public long FileId { get; }

    public long StatesGroupId { get; }

    public long StepsGroupId { get; }

    /// <inheritdoc />
    public void StoreState(ISolutionState state)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void StoreStep(ISolutionStep step)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Hdf5.CloseFile(FileId);
    }
}