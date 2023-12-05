using MathNet.Numerics;
using RefraSin.ParticleModel.ParticleFactories;
using static NUnit.Framework.Assert;

namespace RefraSin.ParticleModel.Test;

public class TestSpecs
{
    private IParticle _particle;

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
        That((double)_particle.Nodes[^1].Coordinates.Phi - Constants.Pi2, Is.Not.EqualTo(0.0));
    }

    [Test]
    public void TestIndexerInt()
    {
        That(_particle[0], Is.EqualTo(_particle.Nodes[0]));
        That(_particle[1], Is.EqualTo(_particle.Nodes[1]));
        That(_particle[-1], Is.EqualTo(_particle.Nodes[^1]));
        That(_particle[_particle.Nodes.Count], Is.EqualTo(_particle.Nodes[0]));
    }

    [Test]
    public void TestIndexerGuid()
    {
        That(_particle[_particle.Nodes[0].Id], Is.EqualTo(_particle.Nodes[0]));
        That(_particle[_particle.Nodes[1].Id], Is.EqualTo(_particle.Nodes[1]));
        That(_particle[_particle.Nodes[^1].Id], Is.EqualTo(_particle.Nodes[^1]));

        Throws(
            Is.TypeOf<IndexOutOfRangeException>(),
            () => { _ = _particle[Guid.NewGuid()]; }
        );
    }
}