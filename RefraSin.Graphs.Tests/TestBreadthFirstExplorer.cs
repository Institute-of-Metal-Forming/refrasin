namespace RefraSin.Graphs.Tests;

[TestFixture]
public class TestBreadthFirstExplorer
{
    private Vertex[] _vertices;
    private IEdge<Vertex>[] _edges;
    private DirectedGraph<Vertex> _graph;

    [SetUp]
    public void Setup()
    {
        _vertices = new Vertex[]
        {
            new Vertex(Guid.NewGuid(), "0"),
            new Vertex(Guid.NewGuid(), "1"),
            new Vertex(Guid.NewGuid(), "2"),
            new Vertex(Guid.NewGuid(), "3"),
            new Vertex(Guid.NewGuid(), "4"),
            new Vertex(Guid.NewGuid(), "5"),
        };

        _edges = new IEdge<Vertex>[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], false),
        };
    }

    [Test]
    public void TestOrderDirected()
    {
        var graph = new DirectedGraph<Vertex>(_vertices, _edges);

        var explorer = BreadthFirstExplorer<Vertex>.Explore(graph, _vertices[0]);

        Assert.That(explorer.TraversedEdges, Is.EqualTo(new IEdge[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], true),
            new Edge<Vertex>(_vertices[5], _vertices[2], true),
        }));
    }

    [Test]
    public void TestOrderUndirected()
    {
        var graph = new UndirectedGraph<Vertex>(_vertices, _edges);

        var explorer = BreadthFirstExplorer<Vertex>.Explore(graph, _vertices[0]);

        Assert.That(explorer.TraversedEdges, Is.EqualTo(new IEdge[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[1], _vertices[0], true),
            new Edge<Vertex>(_vertices[4], _vertices[0], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], true),
            new Edge<Vertex>(_vertices[2], _vertices[1], true),
            new Edge<Vertex>(_vertices[3], _vertices[1], true),
            new Edge<Vertex>(_vertices[5], _vertices[2], true),
        }));
    }

    [Test]
    public void TestOrderDirectedFromUndirected()
    {
        var graph = DirectedGraph<Vertex>.FromGraph(new UndirectedGraph<Vertex>(_vertices, _edges));

        var explorer = BreadthFirstExplorer<Vertex>.Explore(graph, _vertices[0]);

        Assert.That(explorer.TraversedEdges, Is.EqualTo(new IEdge[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[1], _vertices[0], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[4], _vertices[0], true),
            new Edge<Vertex>(_vertices[2], _vertices[1], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], true),
            new Edge<Vertex>(_vertices[3], _vertices[1], true),
            new Edge<Vertex>(_vertices[5], _vertices[2], true),
        }));
    }

    [Test]
    public void TestOrderDirectedNoBackstep()
    {
        var graph = new DirectedGraph<Vertex>(_vertices, _edges);

        var explorer = BreadthFirstExplorer<Vertex>.Explore(graph, _vertices[0], false);

        Assert.That(explorer.TraversedEdges, Is.EqualTo(new IEdge[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], true),
        }));
    }

    [Test]
    public void TestOrderUndirectedNoBackstep()
    {
        var graph = new UndirectedGraph<Vertex>(_vertices, _edges);

        var explorer = BreadthFirstExplorer<Vertex>.Explore(graph, _vertices[0], false);

        Assert.That(explorer.TraversedEdges, Is.EqualTo(new IEdge[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], true),
        }));
    }

    [Test]
    public void TestOrderDirectedFromUndirectedNoBackstep()
    {
        var graph = DirectedGraph<Vertex>.FromGraph(new UndirectedGraph<Vertex>(_vertices, _edges));

        var explorer = BreadthFirstExplorer<Vertex>.Explore(graph, _vertices[0], false);

        Assert.That(explorer.TraversedEdges, Is.EqualTo(new IEdge[]
        {
            new Edge<Vertex>(_vertices[0], _vertices[1], true),
            new Edge<Vertex>(_vertices[0], _vertices[4], true),
            new Edge<Vertex>(_vertices[1], _vertices[2], true),
            new Edge<Vertex>(_vertices[1], _vertices[3], true),
            new Edge<Vertex>(_vertices[2], _vertices[5], true),
        }));
    }
}