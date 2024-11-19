namespace RefraSin.Graphs.Tests;

[TestFixture]
public class UndirectedGraphTests
{
    private Vertex[] _vertices;
    private IEdge<Vertex>[] _edges;
    private UndirectedGraph<Vertex> _graph;

    [SetUp]
    public void Setup()
    {
        _vertices = new Vertex[]
        {
            new Vertex(Guid.NewGuid()),
            new Vertex(Guid.NewGuid()),
            new Vertex(Guid.NewGuid()),
        };

        _edges = new IEdge<Vertex>[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], false),
            new Edge<Vertex>(_vertices[1], _vertices[2], false),
            new Edge<Vertex>(_vertices[2], _vertices[0], false),
        };

        _graph = new UndirectedGraph<Vertex>(_vertices, _edges);
    }

    [Test]
    public void TestVertices()
    {
        Assert.That(_graph.Vertices.ToHashSet(), Is.EqualTo(_vertices.ToHashSet()));
    }

    [Test]
    public void TestEdges()
    {
        Assert.That(_graph.Edges.ToHashSet(), Is.EqualTo(_edges.ToHashSet()));
    }

    [Test]
    public void TestChildrenOf()
    {
        Assert.That(
            _graph.ChildrenOf(_vertices[0]).ToHashSet(),
            Is.EqualTo(_vertices[1..].ToHashSet())
        );
    }

    [Test]
    public void TestParentsOf()
    {
        Assert.That(
            _graph.ParentsOf(_vertices[0]).ToHashSet(),
            Is.EqualTo(_vertices[1..].ToHashSet())
        );
    }
}
