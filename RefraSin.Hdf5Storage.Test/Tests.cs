using HDF5CSharp;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using Refrasin.HDF5Storage;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.Storage;
using static NUnit.Framework.Assert;
using PolarPoint = RefraSin.Coordinates.Polar.PolarPoint;

namespace RefraSin.HDF5Storage.Test;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestOpenAndCloseFile()
    {
        var fileName = Path.GetTempFileName();

        var storage = new Hdf5SolutionStorage(fileName);

        TestContext.WriteLine(fileName);
        That(File.Exists(fileName), Is.True);

        storage.Dispose();
    }

    [Test]
    public void TestWriteState()
    {
        var fileName = Path.GetTempFileName().Replace(".tmp", ".h5");
        var storage = new Hdf5SolutionStorage(fileName);

        TestContext.WriteLine(fileName);
        var preFileSize = new FileInfo(fileName).Length;

        var particle = new ShapeFunctionParticleFactory(100, 0.1, 5, 0.1, Guid.NewGuid()).GetParticle();

        storage.StoreState(new SolutionState(
            0.12,
            new[]
            {
                particle
            }
        ));

        That(new FileInfo(fileName), Has.Length.GreaterThan(preFileSize));

        storage.Dispose();
    }

    [Test]
    public void TestWriteStep()
    {
        var fileName = Path.GetTempFileName().Replace(".tmp", ".h5");
        var storage = new Hdf5SolutionStorage(fileName);

        TestContext.WriteLine(fileName);
        var preFileSize = new FileInfo(fileName).Length;

        var particleStep = new ParticleTimeStep(
            Guid.NewGuid(),
            1,
            0.5,
            0.25,
            new[]
            {
                new NodeTimeStep(
                    Guid.NewGuid(),
                    1,
                    2,
                    new ToUpperToLower(1, -1),
                    1.11
                )
            }
        );

        storage.StoreStep(new SolutionStep(
            0,
            0.12,
            new[]
            {
                particleStep
            }
        ));

        That(new FileInfo(fileName), Has.Length.GreaterThan(preFileSize));

        storage.Dispose();
    }
}