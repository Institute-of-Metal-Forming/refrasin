namespace RefraSin.Graphs.Tests;

[TestFixture]
public class DirectedGraphTests
{
    private Vertex[] _vertices;
    private Edge<Vertex>[] _edges;
    private DirectedGraph<Vertex, Edge<Vertex>> _graph;

    [SetUp]
    public void Setup()
    {
        _vertices = [new(Guid.NewGuid()), new(Guid.NewGuid()), new(Guid.NewGuid())];

        _edges =
        [
            new(_vertices[0], _vertices[1]),
            new(_vertices[1], _vertices[2]),
            new(_vertices[2], _vertices[0]),
        ];

        _graph = new(_vertices, _edges);
    }

    [Test]
    public void TestVertices()
    {
        Assert.That(_graph.Vertices.ToHashSet(), Is.EqualTo(_vertices.ToHashSet()));
    }

    [Test]
    public void TestEdges()
    {
        Assert.That(
            _graph.Edges.ToHashSet(),
            Is.EqualTo(
                new IEdge<Vertex>[]
                {
                    new Edge<Vertex>(_vertices[0], _vertices[1]),
                    new Edge<Vertex>(_vertices[1], _vertices[2]),
                    new Edge<Vertex>(_vertices[2], _vertices[1]),
                    new Edge<Vertex>(_vertices[2], _vertices[0]),
                }.ToHashSet()
            )
        );
    }

    [Test]
    public void TestChildrenOf()
    {
        Assert.That(
            _graph.ChildrenOf(_vertices[0]).ToHashSet(),
            Is.EqualTo(_vertices[1..2].ToHashSet())
        );
        Assert.That(
            _graph.ChildrenOf(_vertices[1]).ToHashSet(),
            Is.EqualTo(_vertices[2..].ToHashSet())
        );
    }

    [Test]
    public void TestParentsOf()
    {
        Assert.That(
            _graph.ParentsOf(_vertices[0]).ToHashSet(),
            Is.EqualTo(_vertices[2..].ToHashSet())
        );
        Assert.That(
            _graph.ParentsOf(_vertices[1]).ToHashSet(),
            Is.EqualTo(new[] { _vertices[0], _vertices[2] }.ToHashSet())
        );
    }
}
