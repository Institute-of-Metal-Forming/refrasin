namespace RefraSin.Graphs.Tests;

[TestFixture]
public class UndirectedEdgesTests
{
    private Vertex _vertex1;
    private Vertex _vertex2;
    private UndirectedEdge<Vertex> _edge1;
    private UndirectedEdge<Vertex> _edge2;

    [SetUp]
    public void Setup()
    {
        _vertex1 = new Vertex(Guid.NewGuid());
        _vertex2 = new Vertex(Guid.NewGuid());

        _edge1 = new UndirectedEdge<Vertex>(_vertex1, _vertex2);
        _edge2 = new UndirectedEdge<Vertex>(_vertex2, _vertex1);
    }

    [Test]
    public void TestEquality()
    {
        Assert.That(
            _edge1,
            Is.EqualTo(
                _edge2
            )
        );
        Assert.That(
            _edge2,
            Is.EqualTo(
                _edge1
            )
        );
    }

    [Test]
    public void TestHashCodeEquality()
    {
        Assert.That(
            _edge1.GetHashCode(),
            Is.EqualTo(
                _edge2.GetHashCode()
            )
        );
        Assert.That(
            _edge2.GetHashCode(),
            Is.EqualTo(
                _edge1.GetHashCode()
            )
        );
    }

    [Test]
    public void TestDirected()
    {
        Assert.That(
            !_edge1.IsDirected
        );
    }

    [Test]
    public void TestIsEdgeFrom()
    {
        Assert.That(
            _edge1.IsEdgeFrom(_vertex1)
        );
        Assert.That(
            _edge1.IsEdgeFrom(_vertex2)
        );
    }

    [Test]
    public void TestIsEdgeTo()
    {
        Assert.That(
            _edge1.IsEdgeTo(_vertex1)
        );
        Assert.That(
            _edge1.IsEdgeTo(_vertex2)
        );
    }

    [Test]
    public void TestReversed()
    {
        var reversed = _edge1.Reversed();

        Assert.That(
            reversed,
            Is.EqualTo(
                _edge1
            )
        );
        Assert.That(
            reversed.Start,
            Is.EqualTo(
                _edge1.End
            )
        );
        Assert.That(
            reversed.End,
            Is.EqualTo(
                _edge1.Start
            )
        );
    }
}