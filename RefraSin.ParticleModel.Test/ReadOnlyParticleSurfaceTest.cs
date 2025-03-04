using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class ReadOnlyParticleSurfaceTest
{
    private IReadOnlyParticleSurface<IParticleNode> _surface =
        new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (0, 0),
            0,
            100,
            1,
            0.2,
            5,
            0.2
        )
            .GetParticle()
            .Nodes;

    [Test]
    public void TestNextUpperNodeFrom()
    {
        Assert.That(_surface.NextUpperNodeFrom(0.01), Is.EqualTo(_surface[1]));
        Assert.That(
            _surface.NextUpperNodeFrom(_surface[20].Coordinates.Phi + 0.01),
            Is.EqualTo(_surface[21])
        );
        Assert.That(_surface.NextUpperNodeFrom(-0.01), Is.EqualTo(_surface[0]));
    }

    [Test]
    public void TestNextLowerNodeFrom()
    {
        Assert.That(_surface.NextLowerNodeFrom(0.01), Is.EqualTo(_surface[0]));
        Assert.That(
            _surface.NextLowerNodeFrom(_surface[20].Coordinates.Phi + 0.01),
            Is.EqualTo(_surface[20])
        );
        Assert.That(_surface.NextLowerNodeFrom(-0.01), Is.EqualTo(_surface[-1]));
    }

    [Test]
    [TestCase(0, 3, 4)]
    [TestCase(3, 6, 4)]
    [TestCase(98, 2, 5)]
    [TestCase(21, 21, 1)]
    [TestCase(21, 20, 100)]
    public void TestSliceInt(int start, int end, int expectedLength)
    {
        var slice = _surface[start, end];
        Assert.That(slice.Count, Is.EqualTo(expectedLength));
        Assert.That(slice[0], Is.EqualTo(_surface[start]));
        Assert.That(slice[^1], Is.EqualTo(_surface[end]));
    }
}
