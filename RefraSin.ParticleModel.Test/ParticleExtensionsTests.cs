using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class ParticleExtensionsTests
{
    private readonly Particle<ParticleNode> _particle;

    public ParticleExtensionsTests()
    {
        _particle = new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.Empty).GetParticle();
    }

    public static IEnumerable<TestCaseData> GenerateMayHaveContactToByRectangularApproximationData()
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
                CenterCoordinates = new(0, 0.5)
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
                CenterCoordinates = new(0, -0.5)
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
                CenterCoordinates = new(0.5, 0.5)
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
                CenterCoordinates = new(-0.5, -0.5)
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
                CenterCoordinates = new(-0.5, 0.5)
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
                CenterCoordinates = new(0.5, -0.5)
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
    [TestCaseSource(nameof(GenerateMayHaveContactToByRectangularApproximationData))]
    public void TestMayHaveContactToByRectangularApproximation(
        Particle<ParticleNode> other,
        bool expectedResult
    )
    {
        Assert.That(
            _particle.MayHaveContactToByRectangularApproximation(other),
            Is.EqualTo(expectedResult)
        );
    }

    public static IEnumerable<TestCaseData> GeneratePointIsInParticleData()
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
    }

    [Test]
    [TestCaseSource(nameof(GeneratePointIsInParticleData))]
    public void TestPointIsInParticle(IPoint point, bool expectedResult)
    {
        Assert.That(_particle.PointIsInParticle(point), Is.EqualTo(expectedResult));
    }
}
