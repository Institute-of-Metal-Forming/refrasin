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
        _state =
            new SystemState(Guid.NewGuid(), 0.12, new[] { particle1, particle2 });
    }
    
    [Test]
    public void TestWriteState()
    {
        var fileName = Path.Combine(TempPath.CreateTempDir(), "dump.parquet");
        var storage = new ParquetStorage(fileName);

        var preFileSize = new FileInfo(fileName).Length;

        storage.StoreState(
            null!,
            _state
        );

        storage.Dispose();
        Assert.That(new FileInfo(fileName), Has.Length.GreaterThan(preFileSize));
    }
    
    [Test]
    public void TestCompression()
    {
        var dirName = TempPath.CreateTempDir();
        var fileNameCompressed = Path.Combine(dirName, "compressed.parquet");
        var fileNamePlain = Path.Combine(dirName, "plain.parquet");
        
        var storageCompressed = new ParquetStorage(fileNameCompressed);
        storageCompressed.StoreState(
            null!,
            _state
        );
        storageCompressed.Dispose();
        
        var storagePlain = new ParquetStorage(fileNamePlain);
        storagePlain.StoreState(
            null!,
            _state
        );
        storagePlain.Dispose();
        
        var sizeCompressed = new FileInfo(fileNameCompressed).Length;
        var sizePlain = new FileInfo(fileNamePlain).Length;
        TestContext.WriteLine($"Compressed: {sizeCompressed}");
        TestContext.WriteLine($"Plain: {sizePlain}");
        
        Assert.That(sizeCompressed, Is.LessThan(sizePlain));
    }
}