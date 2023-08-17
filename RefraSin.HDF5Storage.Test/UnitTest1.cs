using HDF5CSharp;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using Refrasin.HDF5Storage;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.Storage;
using static NUnit.Framework.Assert;

namespace RefraSin.HDF5Storage.Test;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestOpenAndCloseFile()
    {
        var fileName = Path.GetTempFileName();

        var storage = new HDF5SolutionStorage(fileName);

        TestContext.WriteLine(fileName);
        That(File.Exists(fileName), Is.True);

        storage.Dispose();
    }

    [Test]
    public void TestWriteState()
    {
        var fileName = Path.GetTempFileName();

        var storage = new HDF5SolutionStorage(fileName);

        TestContext.WriteLine(fileName);
        var fi = new FileInfo(fileName);
        var preFileSize = fi.Length;

        var particleSpec = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, Guid.NewGuid()).GetParticleSpec();
        var particle = new Particle(
            particleSpec.Id,
            new PolarPoint(particleSpec.AbsoluteCenterCoordinates),
            particleSpec.AbsoluteCenterCoordinates,
            particleSpec.RotationAngle,
            particleSpec.MaterialId,
            particleSpec.NodeSpecs.Select(ns => new SurfaceNode(
                ns.Id,
                ns.ParticleId,
                ns.Coordinates,
                ns.Coordinates.Absolute,
                new ToUpperToLower(0, 0),
                new ToUpperToLowerAngle(0, 0),
                new ToUpperToLowerAngle(0, 0),
                new ToUpperToLower(0, 0),
                new NormalTangentialAngle(0, 0),
                new ToUpperToLower(0, 0),
                new ToUpperToLower(0, 0),
                new NormalTangential(0, 0),
                new NormalTangential(0, 0)
            )).ToArray()
        );

        storage.StoreState(new SolutionState(
            0.12,
            new[]
            {
                particle
            }
        ));

        That(fi, Has.Length.GreaterThan(preFileSize));

        storage.Dispose();
    }
}