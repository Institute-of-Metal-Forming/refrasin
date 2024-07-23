using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using static NUnit.Framework.Assert;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.Test;

public class TestSpecs
{
    private IParticle<IParticleNode> _particle;

    [SetUp]
    public void Setup()
    {
        var factory = new ShapeFunctionParticleFactory(
            100,
            0.1,
            5,
            0.1,
            Guid.Empty
        );

        _particle = factory.GetParticle();
    }

    [Test]
    public void TestShapeFunctionSpecFactory()
    {
        That((double)_particle.Nodes[^1].Coordinates.Phi - Constants.TwoPi, Is.Not.EqualTo(0.0));
    }

    [Test]
    public void TestIndexerInt()
    {
        That(_particle.Nodes[-1], Is.EqualTo(_particle.Nodes[^1]));
        That(_particle.Nodes[-2], Is.EqualTo(_particle.Nodes[^2]));
        That(_particle.Nodes[_particle.Nodes.Count], Is.EqualTo(_particle.Nodes[0]));
        That(_particle.Nodes[_particle.Nodes.Count + 1], Is.EqualTo(_particle.Nodes[1]));
    }

    [Test]
    public void TestIndexerGuid()
    {
        That(_particle.Nodes[_particle.Nodes[0].Id], Is.EqualTo(_particle.Nodes[0]));
        That(_particle.Nodes[_particle.Nodes[1].Id], Is.EqualTo(_particle.Nodes[1]));
        That(_particle.Nodes[_particle.Nodes[^1].Id], Is.EqualTo(_particle.Nodes[^1]));

        Throws(
            Is.TypeOf<KeyNotFoundException>(),
            () => { _ = _particle.Nodes[Guid.NewGuid()]; }
        );
    }
}