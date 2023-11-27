using MoreLinq.Extensions;

namespace RefraSin.Graphs.Tests;

[TestFixture]
public class TestDepthFirstPathFinder
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
            new DirectedEdge<Vertex>(_vertices[0], _vertices[1]),
            new DirectedEdge<Vertex>(_vertices[0], _vertices[2]),
            new DirectedEdge<Vertex>(_vertices[1], _vertices[3]),
            new DirectedEdge<Vertex>(_vertices[1], _vertices[4]),
            new DirectedEdge<Vertex>(_vertices[3], _vertices[5]),
            new DirectedEdge<Vertex>(_vertices[5], _vertices[2]),
        };
    }

    [Test]
    public void TestOrderDirected()
    {
        var graph = new DirectedGraph<Vertex>(_vertices, _edges);

        var finder = DepthFirstPathFinder<Vertex>.FindPath(graph, _vertices[0], _vertices[5]);

        Assert.That(
            finder.ExploredEdges.ToArray(),
            Is.EqualTo(
                new[]
                {
                    new DirectedEdge<Vertex>(_vertices[0], _vertices[1]),
                    new DirectedEdge<Vertex>(_vertices[1], _vertices[3]),
                    new DirectedEdge<Vertex>(_vertices[3], _vertices[5]),
                }
            )
        );
    }

    [Test]
    public void TestOrderUndirected()
    {
        var graph = new UndirectedGraph<Vertex>(_vertices, _edges.Shuffle());

        var finder = DepthFirstPathFinder<Vertex>.FindPath(graph, _vertices[0], _vertices[5]);

        Assert.That(
            finder.ExploredEdges.ToArray(),
            Is.EqualTo(
                new[]
                {
                    new DirectedEdge<Vertex>(_vertices[0], _vertices[1]),
                    new DirectedEdge<Vertex>(_vertices[1], _vertices[3]),
                    new DirectedEdge<Vertex>(_vertices[3], _vertices[5]),
                }
            ).Or.EqualTo(
                new[]
                {
                    new DirectedEdge<Vertex>(_vertices[0], _vertices[2]),
                    new DirectedEdge<Vertex>(_vertices[2], _vertices[5]),
                }
            )
        );
    }
}