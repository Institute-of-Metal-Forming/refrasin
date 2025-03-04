using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class ParticleSurfaceTest
{
    public ParticleSurfaceTest()
    {
        _particle = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (0, 0),
            0,
            100,
            1,
            0.2,
            5,
            0.2
        ).GetParticle();
    }

    [SetUp]
    public void Setup()
    {
        _surface = new ParticleSurface<IParticleNode>(_particle.Nodes);
        _index = 21;
        _nodeAtIndex = _surface[_index];
        _nodeAboveIndex = _surface[_index + 1];
        _nodeBelowIndex = _surface[_index - 1];
    }

    private IParticleSurface<IParticleNode> _surface;
    private Particle<ParticleNode> _particle;
    private int _index;
    private IParticleNode _nodeAtIndex;
    private IParticleNode _nodeAboveIndex;
    private IParticleNode _nodeBelowIndex;

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
    public void TestIndexOf()
    {
        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(_nodeAtIndex), Is.EqualTo(_index));
        Assert.That(_surface.IndexOf(_nodeAboveIndex), Is.EqualTo(_index + 1));
    }

    [Test]
    public void TestInsertAbove()
    {
        var node = new ParticleNode(Guid.NewGuid(), _particle, new PolarPoint(), NodeType.Surface);
        _surface.InsertAbove(_nodeAtIndex, node);

        Assert.That(_surface.IndexOf(node), Is.EqualTo(_index + 1));

        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(_nodeAtIndex), Is.EqualTo(_index));
        Assert.That(_surface.IndexOf(_nodeAboveIndex), Is.EqualTo(_index + 2));
    }

    [Test]
    public void TestInsertAboveMultiple([Values(1, 2, 3)] int count)
    {
        var nodes = Enumerable
            .Repeat(0, count)
            .Select(i => new ParticleNode(
                Guid.NewGuid(),
                _particle,
                new PolarPoint(),
                NodeType.Surface
            ))
            .ToArray();
        _surface.InsertAbove(_nodeAtIndex, nodes);

        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(_nodeAtIndex), Is.EqualTo(_index));
        Assert.That(_surface.IndexOf(_nodeAboveIndex), Is.EqualTo(_index + count + 1));
        Assert.That(_surface.Count, Is.EqualTo(100 + count));
        Assert.That(_surface.Skip(_index + 1).Take(count), Is.EqualTo(nodes));
    }

    [Test]
    public void TestInsertBelow()
    {
        var node = new ParticleNode(Guid.NewGuid(), _particle, new PolarPoint(), NodeType.Surface);
        _surface.InsertBelow(_nodeAtIndex, node);

        Assert.That(_surface.IndexOf(node), Is.EqualTo(_index));

        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(_nodeAtIndex), Is.EqualTo(_index + 1));
        Assert.That(_surface.IndexOf(_nodeAboveIndex), Is.EqualTo(_index + 2));
    }

    [Test]
    public void TestInsertBelowMultiple([Values(1, 2, 3)] int count)
    {
        var nodes = Enumerable
            .Repeat(0, count)
            .Select(i => new ParticleNode(
                Guid.NewGuid(),
                _particle,
                new PolarPoint(),
                NodeType.Surface
            ))
            .ToArray();
        _surface.InsertBelow(_nodeAtIndex, nodes);

        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(_nodeAtIndex), Is.EqualTo(_index + count));
        Assert.That(_surface.IndexOf(_nodeAboveIndex), Is.EqualTo(_index + count + 1));
        Assert.That(_surface.Count, Is.EqualTo(100 + count));
        Assert.That(_surface.Skip(_index).Take(count), Is.EqualTo(nodes));
    }

    [Test]
    public void TestRemove()
    {
        _surface.Remove(_nodeAtIndex);

        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(_nodeAboveIndex), Is.EqualTo(_index));
        Assert.Throws<KeyNotFoundException>(() => _surface.IndexOf(_nodeAtIndex));
    }

    [Test]
    public void TestRemoveMultiple([Values(1, 2, 3)] int count)
    {
        var firstRemaining = _surface[_index + count];
        _surface.Remove(_index, _index + (count - 1));

        Assert.That(_surface.IndexOf(_nodeBelowIndex), Is.EqualTo(_index - 1));
        Assert.That(_surface.IndexOf(firstRemaining), Is.EqualTo(_index));
        Assert.Throws<KeyNotFoundException>(() => _surface.IndexOf(_nodeAtIndex));
        Assert.That(_surface.Count, Is.EqualTo(100 - count));
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
