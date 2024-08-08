using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using ScottPlot;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class ParticleExtensionsTests
{
    private static readonly Particle<ParticleNode> Particle = new ShapeFunctionParticleFactory(
        1,
        0.2,
        5,
        0.2,
        Guid.Empty
    ).GetParticle();

    private readonly string _tempDir;

    public ParticleExtensionsTests()
    {
        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
    }

    public static IEnumerable<TestCaseData> GenerateMayHasContactToByRectangularApproximationData()
    {
        // self
        yield return new TestCaseData(Particle, true);

        // right away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(3, 0)
            }.GetParticle(),
            false
        );

        // right overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(2, 0)
            }.GetParticle(),
            true
        );

        // left away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, 0)
            }.GetParticle(),
            false
        );

        // left overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-2, 0)
            }.GetParticle(),
            true
        );

        // above away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 2)
            }.GetParticle(),
            false
        );

        // above overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 1)
            }.GetParticle(),
            true
        );

        // below away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, -2)
            }.GetParticle(),
            false
        );

        // below overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, -1)
            }.GetParticle(),
            true
        );

        // above right away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(3, 3)
            }.GetParticle(),
            false
        );

        // above right overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(1, 1)
            }.GetParticle(),
            true
        );

        // below left away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, -3)
            }.GetParticle(),
            false
        );

        // below left overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-1, -1)
            }.GetParticle(),
            true
        );

        // above left away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, 3)
            }.GetParticle(),
            false
        );

        // above left overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-1, 1)
            }.GetParticle(),
            true
        );

        // below right away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, -3)
            }.GetParticle(),
            false
        );

        // below right overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(1, -1)
            }.GetParticle(),
            true
        );

        // within
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(0.1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 0)
            }.GetParticle(),
            true
        );

        // around
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(10, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 0)
            }.GetParticle(),
            true
        );
    }

    [Test]
    [TestCaseSource(nameof(GenerateMayHasContactToByRectangularApproximationData))]
    public void TestMayHasContactToByRectangularApproximation(
        Particle<ParticleNode> other,
        bool expectedResult
    )
    {
        Assert.That(
            Particle.MayHasContactToByRectangularApproximation(other),
            Is.EqualTo(expectedResult)
        );
        Assert.That(
            other.MayHasContactToByRectangularApproximation(Particle),
            Is.EqualTo(expectedResult)
        );
    }

    public static IEnumerable<TestCaseData> GenerateContainsPointData()
    {
        yield return new TestCaseData(new AbsolutePoint(0, 0), true);
        yield return new TestCaseData(new AbsolutePoint(1, 0), true);
        yield return new TestCaseData(new AbsolutePoint(1.5, 0), false);
        yield return new TestCaseData(new AbsolutePoint(3, 0), false);
        yield return new TestCaseData(new AbsolutePoint(0, 0.5), true);
        yield return new TestCaseData(new AbsolutePoint(0, 1), false);
        yield return new TestCaseData(new AbsolutePoint(-0.9, 0), true);

        yield return new TestCaseData(new PolarPoint(0.01, 1.4), false);
        yield return new TestCaseData(new PolarPoint(0.01, 1.38), true);

        yield return new TestCaseData(new PolarPoint(-0.01, 1.4), false);
        yield return new TestCaseData(new PolarPoint(-0.01, 1.38), true);

        // own nodes
        yield return new TestCaseData(Particle.Nodes[0].Coordinates, true);
        yield return new TestCaseData(Particle.Nodes[2].Coordinates, true);
        yield return new TestCaseData(Particle.Nodes[20].Coordinates, true);
        yield return new TestCaseData(Particle.Nodes[-1].Coordinates, true);
    }

    [Test]
    [TestCaseSource(nameof(GenerateContainsPointData))]
    public void TestContainsPoint(IPoint point, bool expectedResult)
    {
        Assert.That(Particle.ContainsPoint(point), Is.EqualTo(expectedResult));
    }

    public static IEnumerable<TestCaseData> GenerateHasContactToData()
    {
        // right away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(3, 0)
            }.GetParticle(),
            false
        );

        // right overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(2, 0)
            }.GetParticle(),
            true
        );

        // left away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, 0)
            }.GetParticle(),
            false
        );

        // left overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-2, 0)
            }.GetParticle(),
            true
        );

        // above away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 2)
            }.GetParticle(),
            false
        );

        // above overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 1)
            }.GetParticle(),
            true
        );

        // below away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, -2)
            }.GetParticle(),
            false
        );

        // below overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, -1)
            }.GetParticle(),
            true
        );

        // above right away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(3, 3)
            }.GetParticle(),
            false
        );

        // above right close
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(1.95, 1.95)
            }.GetParticle(),
            false
        );

        // above right overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(1, 1)
            }.GetParticle(),
            true
        );

        // below left away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, -3)
            }.GetParticle(),
            false
        );

        // below left overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-1, -1)
            }.GetParticle(),
            true
        );

        // above left away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, 3)
            }.GetParticle(),
            false
        );

        // above left overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-1, 1)
            }.GetParticle(),
            true
        );

        // below right away
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-3, -3)
            }.GetParticle(),
            false
        );

        // below right overlap
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(1, -1)
            }.GetParticle(),
            true
        );

        // within
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(0.1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 0)
            }.GetParticle(),
            true
        );

        // around
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(10, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(0, 0)
            }.GetParticle(),
            true
        );
    }

    [Test]
    [TestCaseSource(nameof(GenerateHasContactToData))]
    public void TestHasContactTo(Particle<ParticleNode> other, bool expectedResult)
    {
        Assert.That(Particle.HasContactTo(other), Is.EqualTo(expectedResult));
        Assert.That(other.HasContactTo(Particle), Is.EqualTo(expectedResult));
    }

    public static IEnumerable<TestCaseData> GenerateIntersectionPointsToData()
    {
        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(2.36, 0),
                RotationAngle = Angle.Straight
            }.GetParticle(),
            new AbsolutePoint[] { new(1.18, -0.317), new(1.18, 0.317), }
        );

        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(-2.06, 0),
                RotationAngle = Angle.Straight
            }.GetParticle(),
            new AbsolutePoint[]
            {
                new(-1.03, 0.732),
                new(-1.03, 0.1485),
                new(-1.03, -0.1485),
                new(-1.03, -0.732),
            }
        );

        yield return new TestCaseData(
            new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty)
            {
                CenterCoordinates = new(3, 0),
                RotationAngle = Angle.Straight
            }.GetParticle(),
            Array.Empty<AbsolutePoint>()
        );
    }

    [Test]
    [TestCaseSource(nameof(GenerateIntersectionPointsToData))]
    public void TestIntersectionPointsTo(
        IParticle<IParticleNode> other,
        IReadOnlyList<AbsolutePoint> expectedIntersections
    )
    {
        var intersections = Particle.IntersectionPointsTo(other).ToArray();

        var plot = new Plot();
        plot.Axes.SquareUnits();

        plot.PlotParticle(Particle);
        plot.PlotParticle(other);

        foreach (var intersection in intersections)
        {
            plot.Add.Scatter(
                new ScottPlot.Coordinates(intersection.Absolute.X, intersection.Absolute.Y),
                Colors.Magenta
            );
        }

        plot.SavePng(
            Path.Combine(
                _tempDir,
                $"{nameof(TestIntersectionPointsTo)}{TestContext.CurrentContext.Test.ID}.png"
            ),
            1000,
            1000
        );

        Assert.That(
            intersections.Select(p => p.Absolute.X),
            Is.EqualTo(expectedIntersections.Select(p => p.X)).Within(1e-3)
        );
        Assert.That(
            intersections.Select(p => p.Absolute.Y),
            Is.EqualTo(expectedIntersections.Select(p => p.Y)).Within(1e-3)
        );
    }

    [Test]
    [TestCaseSource(nameof(GenerateIntersectionPointsToData))]
    public void TestCreateGrainBoundariesAtIntersections(
        IParticle<IParticleNode> other,
        IReadOnlyList<AbsolutePoint> expectedIntersections
    )
    {
        var newParticles = Particle.CreateGrainBoundariesAtIntersections(other);

        var plot = new Plot();
        plot.Axes.SquareUnits();

        plot.PlotParticle(Particle);
        plot.PlotParticle(other);
        plot.PlotParticle(newParticles.self);
        plot.PlotParticle(newParticles.other);

        plot.SavePng(
            Path.Combine(
                _tempDir,
                $"{nameof(TestCreateGrainBoundariesAtIntersections)}{TestContext.CurrentContext.Test.ID}.png"
            ),
            10000,
            10000
        );
    }
}
