using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Absolute;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class StepVectorMapTest
{
    private IParticle _particleSpec1;
    private IParticle _particleSpec2;
    private StepVectorMap _map;

    [SetUp]
    public void Setup()
    {
        _particleSpec1 = new ShapeFunctionParticleFactory(100, 0.1, 5, 0.1, Guid.Empty).GetParticle();
        _particleSpec2 = new ShapeFunctionParticleFactory(100, 0.1, 5, 0.1, Guid.Empty)
                { CenterCoordinates = new AbsolutePoint(200, 0) }
            .GetParticle();
        var particles = new[] { _particleSpec1, _particleSpec2 };

        _map = new StepVectorMap(particles, particles.SelectMany(p => p.Nodes));
    }

    [Test]
    public void TestGlobalIndices()
    {
        Assert.That(_map.GetIndex(GlobalUnknown.Lambda1), Is.EqualTo(0));
    }

    [Test]
    public void TestNodeIndices()
    {
        // first particle

        // first node
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[0].Id, NodeUnknown.NormalDisplacement), Is.EqualTo(1));
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[0].Id, NodeUnknown.FluxToUpper), Is.EqualTo(2));
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[0].Id, NodeUnknown.Lambda2), Is.EqualTo(3));

        // second node
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[1].Id, NodeUnknown.NormalDisplacement), Is.EqualTo(4));
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[1].Id, NodeUnknown.FluxToUpper), Is.EqualTo(5));
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[1].Id, NodeUnknown.Lambda2), Is.EqualTo(6));

        // last node
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[99].Id, NodeUnknown.NormalDisplacement), Is.EqualTo(298));
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[99].Id, NodeUnknown.FluxToUpper), Is.EqualTo(299));
        Assert.That(_map.GetIndex(_particleSpec1.Nodes[99].Id, NodeUnknown.Lambda2), Is.EqualTo(300));

        // second particle

        // first node
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[0].Id, NodeUnknown.NormalDisplacement), Is.EqualTo(301));
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[0].Id, NodeUnknown.FluxToUpper), Is.EqualTo(302));
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[0].Id, NodeUnknown.Lambda2), Is.EqualTo(303));

        // second node
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[1].Id, NodeUnknown.NormalDisplacement), Is.EqualTo(304));
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[1].Id, NodeUnknown.FluxToUpper), Is.EqualTo(305));
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[1].Id, NodeUnknown.Lambda2), Is.EqualTo(306));

        // last node
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[99].Id, NodeUnknown.NormalDisplacement), Is.EqualTo(598));
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[99].Id, NodeUnknown.FluxToUpper), Is.EqualTo(599));
        Assert.That(_map.GetIndex(_particleSpec2.Nodes[99].Id, NodeUnknown.Lambda2), Is.EqualTo(600));
    }
}