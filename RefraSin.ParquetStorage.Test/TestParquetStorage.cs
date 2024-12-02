using Parquet;
using Parquet.Serialization;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ProcessModel;

namespace RefraSin.ParquetStorage.Test;

public class Tests
{
    private readonly SystemState _state;

    public Tests()
    {
        var particle1 = new ShapeFunctionParticleFactory(
            100,
            0.1,
            5,
            0.1,
            Guid.NewGuid()
        ).GetParticle();
        var particle2 = new ShapeFunctionParticleFactory(100, 0.1, 5, 0.1, Guid.NewGuid())
        {
            CenterCoordinates = new AbsolutePoint(240, 0),
            RotationAngle = Math.PI,
        }.GetParticle();
        _state = new SystemState(Guid.NewGuid(), 0.12, new[] { particle1, particle2 });
    }

    [Test]
    public void TestWriteState()
    {
        var fileName = Path.Combine(TempPath.CreateTempDir(), "dump.parquet");
        var storage = new ParquetStorage(fileName);

        var preFileSize = new FileInfo(fileName).Length;

        storage.StoreState(null!, _state);

        storage.Dispose();
        Assert.That(new FileInfo(fileName), Has.Length.GreaterThan(preFileSize));
    }

    [Test]
    public void TestCompression()
    {
        var compressions = new[]
        {
            CompressionMethod.None,
            CompressionMethod.Snappy,
            CompressionMethod.Gzip,
            CompressionMethod.Lzo,
            CompressionMethod.Brotli,
            CompressionMethod.Zstd,
        };

        var dirName = TempPath.CreateTempDir();
        foreach (var compression in compressions)
        {
            var fileName = Path.Combine(dirName, $"{compression}.parquet");
            var storage = new ParquetStorage(
                fileName,
                options: new ParquetSerializerOptions() { CompressionMethod = compression }
            );
            storage.StoreState(null!, _state);
            storage.Dispose();
            var size = new FileInfo(fileName).Length;
            TestContext.WriteLine($"{compression}: {size}");
        }
    }
}
