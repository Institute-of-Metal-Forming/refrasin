using Parquet;
using Parquet.Serialization;
using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Extensions;

namespace RefraSin.ParquetStorage.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class Tests(ISystemState<IParticle<IParticleNode>, IParticleNode> state)
{
    public static IEnumerable<TestFixtureData> GetTestFixtureData()
    {
        yield return new TestFixtureData(StateWithoutPores()) { TestName = "without pores" };
        yield return new TestFixtureData(StateWithPores()) { TestName = "with pores" };
    }

    static ISystemState<IParticle<IParticleNode>, IParticleNode> StateWithoutPores()
    {
        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            new AbsolutePoint(),
            0,
            100,
            1,
            0.2,
            5,
            0.2
        ).GetParticle();
        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            new AbsolutePoint(240, 0),
            Math.PI,
            100,
            1,
            0.2,
            5,
            0.2
        ).GetParticle();
        return new SystemState(Guid.NewGuid(), 0.12, new[] { particle1, particle2 });
    }

    static ISystemState<IParticle<IParticleNode>, IParticleNode> StateWithPores()
    {
        var nodeCountPerParticle = 100;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (0, -110e-6),
            Angle.Right,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (105e-6, 110e-6),
            Angle.Right + Angle.FromDegrees(120),
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var particle3 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (-105e-6, 110e-6),
            Angle.Right - Angle.FromDegrees(120),
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2, particle3]);
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 2e-6, 1.5e-6).Solve(
            initialState
        );

        var stateWithPores = new SystemState(Guid.Empty, 0, compactedState)
            .DetectPores(0.2, 0)
            .WithoutOuterSurface();

        return stateWithPores;
    }

    [Test]
    public void TestWriteState()
    {
        var fileName = Path.Combine(TempPath.CreateTempDir(), "dump.parquet");
        var storage = new ParquetStorage(fileName, bufferSize: 70);

        var preFileSize = new FileInfo(fileName).Length;

        storage.StoreState(null!, state);
        storage.StoreState(null!, state);

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
            storage.StoreState(null!, state);
            storage.Dispose();
            var size = new FileInfo(fileName).Length;
            TestContext.WriteLine($"{compression}: {size}");
        }
    }
}
