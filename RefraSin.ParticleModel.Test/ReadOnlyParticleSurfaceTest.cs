using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class ReadOnlyParticleSurfaceTest
{
    private IReadOnlyParticleSurface<IParticleNode> _surface = new ShapeFunctionParticleFactory(
        1,
        0.2,
        5,
        0.1,
        Guid.Empty
    )
    {
        NodeCount = 100
    }
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
}
