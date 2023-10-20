using MathNet.Numerics;
using RefraSin.ParticleModel.ParticleSpecFactories;
using static NUnit.Framework.Assert;

namespace RefraSin.ParticleModel.Test;

public class TestSpecs
{
    private IParticleSpec _spec;

    [SetUp]
    public void Setup()
    {
        var factory = new ShapeFunctionParticleSpecFactory(
            100,
            0.1,
            5,
            0.1,
            Guid.Empty
        );

        _spec = factory.GetParticleSpec();
    }

    [Test]
    public void TestShapeFunctionSpecFactory()
    {
        That((double)_spec.NodeSpecs[^1].Coordinates.Phi - Constants.Pi2, Is.Not.EqualTo(0.0));
    }

    [Test]
    public void TestIndexerInt()
    {
        That(_spec[0], Is.EqualTo(_spec.NodeSpecs[0]));
        That(_spec[1], Is.EqualTo(_spec.NodeSpecs[1]));
        That(_spec[-1], Is.EqualTo(_spec.NodeSpecs[^1]));
        That(_spec[_spec.NodeSpecs.Count], Is.EqualTo(_spec.NodeSpecs[0]));
    }

    [Test]
    public void TestIndexerGuid()
    {
        That(_spec[_spec.NodeSpecs[0].Id], Is.EqualTo(_spec.NodeSpecs[0]));
        That(_spec[_spec.NodeSpecs[1].Id], Is.EqualTo(_spec.NodeSpecs[1]));
        That(_spec[_spec.NodeSpecs[^1].Id], Is.EqualTo(_spec.NodeSpecs[^1]));

        Throws(
            Is.TypeOf<IndexOutOfRangeException>(),
            () => { _ = _spec[Guid.NewGuid()]; }
        );
    }
}