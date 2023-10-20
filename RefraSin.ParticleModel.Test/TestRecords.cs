using MathNet.Numerics;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.ParticleSpecFactories;
using static NUnit.Framework.Assert;

namespace RefraSin.ParticleModel.Test;

public class TestRecords
{
    private IParticleSpec _spec;
    private IParticle _particle;

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

        _particle = new Particle(
            _spec.Id,
            new PolarPoint(_spec.AbsoluteCenterCoordinates),
            _spec.AbsoluteCenterCoordinates,
            _spec.RotationAngle,
            _spec.MaterialId,
            _spec.NodeSpecs.Select(n => (INode)new SurfaceNode(
                n.Id,
                n.ParticleId,
                n.Coordinates,
                n.Coordinates.Absolute,
                0, 0, 0, 0, 0, 0, 0, 0, 0
            )).ToArray()
        );
    }

    [Test]
    public void TestShapeFunctionSpecFactory()
    {
        That((double)_particle.NodeSpecs[^1].Coordinates.Phi - Constants.Pi2, Is.Not.EqualTo(0.0));
    }

    [Test]
    public void TestIndexerInt()
    {
        That(_particle[0], Is.EqualTo(_particle.NodeSpecs[0]));
        That(_particle[1], Is.EqualTo(_particle.NodeSpecs[1]));
        That(_particle[-1], Is.EqualTo(_particle.NodeSpecs[^1]));
        That(_particle[_particle.NodeSpecs.Count], Is.EqualTo(_particle.NodeSpecs[0]));
    }

    [Test]
    public void TestIndexerGuid()
    {
        That(_particle[_particle.NodeSpecs[0].Id], Is.EqualTo(_particle.NodeSpecs[0]));
        That(_particle[_particle.NodeSpecs[1].Id], Is.EqualTo(_particle.NodeSpecs[1]));
        That(_particle[_particle.NodeSpecs[^1].Id], Is.EqualTo(_particle.NodeSpecs[^1]));

        Throws(
            Is.TypeOf<IndexOutOfRangeException>(),
            () => { _ = _particle[Guid.NewGuid()]; }
        );
    }
}