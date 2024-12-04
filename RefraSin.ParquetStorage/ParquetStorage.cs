using Parquet;
using Parquet.Serialization;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.Storage;

namespace RefraSin.ParquetStorage;

public class ParquetStorage(
    string fileName,
    int bufferSize = 1_000,
    ParquetSerializerOptions? options = null
) : ISolutionStorage, IDisposable
{
    private readonly FileStream _stream = new(
        fileName,
        FileMode.OpenOrCreate,
        FileAccess.ReadWrite
    );
    private readonly List<Row> _rowBuffer = new(2 * bufferSize);
    private readonly ParquetSerializerOptions _options = options ?? new ParquetSerializerOptions();

    /// <inheritdoc />
    public void StoreState(
        IProcessStep processStep,
        ISystemState<IParticle<IParticleNode>, IParticleNode> state
    )
    {
        var stateData = StateData.From(state);

        foreach (var particle in state.Particles)
        {
            var particleData = ParticleData.From(particle);

            foreach (var node in particle.Nodes)
            {
                var nodeData = NodeData.From(node);
                _rowBuffer.Add(
                    new Row
                    {
                        State = stateData,
                        Particle = particleData,
                        Node = nodeData,
                    }
                );
            }
        }

        if (_rowBuffer.Count > bufferSize)
        {
            WriteRowBuffer();
        }
    }

    private void WriteRowBuffer()
    {
        var data = _rowBuffer.ToArray();
        _rowBuffer.Clear();
        Task.Run(() => ParquetSerializer.SerializeAsync(data, _stream, _options));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_rowBuffer.Count != 0)
        {
            WriteRowBuffer();
        }

        _stream.Dispose();
    }
}
