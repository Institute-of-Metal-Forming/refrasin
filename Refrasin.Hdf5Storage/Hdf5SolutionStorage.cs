using HDF5CSharp;
using HDF5CSharp.DataTypes;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;

namespace Refrasin.HDF5Storage;

public class Hdf5SolutionStorage : ISolutionStorage, IDisposable
{
    public Hdf5SolutionStorage(
        string filePath,
        string statesGroupName = "States"
    )
    {
        FilePath = filePath;
        StatesGroupName = statesGroupName;
        FileId = Hdf5.CreateFile(FilePath);
        StatesGroupId = Hdf5.CreateOrOpenGroup(FileId, statesGroupName);
    }

    public string FilePath { get; }
    public string StatesGroupName { get; }

    public long FileId { get; }

    public long StatesGroupId { get; }

    private int _stateIndex = 0;

    /// <inheritdoc />
    public void StoreState(IProcessStep processStep, ISystemState<IParticle<IParticleNode>, IParticleNode> state)
    {
        var stateId = Hdf5.CreateOrOpenGroup(StatesGroupId, _stateIndex.ToString());
        Hdf5.WriteAttribute(stateId, nameof(state.Time), state.Time);

        Hdf5.WriteCompounds(
            stateId,
            "Particles",
            state.Particles.Select(p => new ParticleCompound(p)).ToArray(),
            new Dictionary<string, List<string>>()
        );

        var nodesGroupId = Hdf5.CreateOrOpenGroup(stateId, "Nodes");

        foreach (var particleState in state.Particles)
        {
            Hdf5.WriteCompounds(
                nodesGroupId,
                particleState.Id.ToString(),
                particleState.Nodes.Select(n => new NodeCompound(n)).ToArray(),
                new Dictionary<string, List<string>>()
            );
        }

        _stateIndex += 1;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Hdf5.CloseFile(FileId);
    }
}