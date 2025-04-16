using Parquet;
using Parquet.Schema;
using Parquet.Serialization;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using Serilog;

namespace RefraSin.ParquetStorage;

public class ParquetStorage(
    string fileName,
    int bufferSize = 10_000,
    ParquetSerializerOptions options = null
) : ISolutionStorage, IDisposable
{
    private readonly FileStream _stream = new(fileName, FileMode.Create, FileAccess.ReadWrite);
    private bool _append = false;
    private readonly List<Row> _rowBuffer = new(2 * bufferSize);
    private Task _writeTask;
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
            _writeTask?.Wait();
            _writeTask = WriteRowBufferAsync();
        }
    }

    private async Task WriteRowBufferAsync()
    {
        var logger = Log.ForContext<ParquetStorage>();
        logger.Information("Writing buffered data to Parquet file...");
        var data = _rowBuffer.ToArray();
        _rowBuffer.Clear();
        _options.Append = _append;
        await ParquetSerializer.SerializeAsync(data, _stream, _options);
        _append = true;
        logger.Information("Writing finished.");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _writeTask?.Wait();

        if (_rowBuffer.Count != 0)
        {
            WriteRowBufferAsync().Wait();
        }

        _stream.Dispose();
    }
}
