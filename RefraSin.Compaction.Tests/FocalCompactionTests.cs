using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using ScottPlot;
using static RefraSin.Coordinates.Angle;

namespace RefraSin.Compaction.Tests;

[TestFixture]
[TestFixtureSource(nameof(GenerateFixtureData))]
public class FocalCompactionTests
{
    private readonly AbsolutePoint _focus;
    private readonly SystemState _system;
    private readonly string _tempDir;

    public static IEnumerable<TestFixtureData> GenerateFixtureData()
    {
        yield return new TestFixtureData(
            new []
            {
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty).GetParticle(),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (3,0), RotationAngle = Straight}.GetParticle()
            },
            new AbsolutePoint(0,0)
        );
        yield return new TestFixtureData(
            new []
            {
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty).GetParticle(),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (-3,0), RotationAngle = Straight}.GetParticle()
            },
            new AbsolutePoint(0,0)
        );
        yield return new TestFixtureData(
            new []
            {
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (-3,-3), RotationAngle = HalfRight}.GetParticle(),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (3,-3), RotationAngle = Straight - HalfRight}.GetParticle()
            },
            new AbsolutePoint(0,0)
        );
        yield return new TestFixtureData(
            new []
            {
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (3,6), RotationAngle = HalfRight}.GetParticle(),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (-3,6), RotationAngle = Straight - HalfRight}.GetParticle(),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (3,-6), RotationAngle = -HalfRight}.GetParticle(),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty){CenterCoordinates = (-3,-6), RotationAngle = Straight + HalfRight}.GetParticle()
            },
            new AbsolutePoint(0,0)
        );
    }
    
    public FocalCompactionTests(IEnumerable<Particle<ParticleNode>> particles, AbsolutePoint focus)
    {
        _focus = focus;
        _system = new SystemState(Guid.Empty, 0, particles);
        
        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
    }

    [Test]
    [TestCase(0.05)]
    [TestCase(0.10)]
    public void TestCompaction(double stepSize)
    {
        var compactor = new FocalCompactionStep(_focus, stepSize);
        var sol = compactor.Solve(_system);

        var plot = new Plot();
        plot.Axes.SquareUnits();

        foreach (var particle in sol.Particles)
        {
           plot.PlotParticle(particle); 
        }
        
        plot.SavePng(
            Path.Combine(
                _tempDir,
                $"{nameof(TestCompaction)}{TestContext.CurrentContext.Test.ID}.png"
            ),
            10000,
            10000
        );
    }
}