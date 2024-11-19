using MoreLinq.Extensions;

namespace RefraSin.Graphs.Tests;

[TestFixture]
public class TestDepthFirstPathFinder
{
    private Vertex[] _vertices;
    private Edge<Vertex>[] _edges;

    [SetUp]
    public void Setup()
    {
        _vertices =
        [
            new(Guid.NewGuid(), "0"),
            new(Guid.NewGuid(), "1"),
            new(Guid.NewGuid(), "2"),
            new(Guid.NewGuid(), "3"),
            new(Guid.NewGuid(), "4"),
            new(Guid.NewGuid(), "5"),
        ];

        _edges =
        [
            new Edge<Vertex>(_vertices[0], _vertices[1]),
            new Edge<Vertex>(_vertices[0], _vertices[2]),
            new Edge<Vertex>(_vertices[1], _vertices[3]),
            new Edge<Vertex>(_vertices[1], _vertices[4]),
            new Edge<Vertex>(_vertices[3], _vertices[5]),
            new Edge<Vertex>(_vertices[5], _vertices[2]),
        ];
    }

    [Test]
    public void TestOrderDirected()
    {
        var graph = new DirectedGraph<Vertex, Edge<Vertex>>(_vertices, _edges.Shuffle());

        var finder = DepthFirstPathFinder<Vertex, Edge<Vertex>>.FindPath(
            graph,
            _vertices[0],
            _vertices[5]
        );

        Assert.That(
            finder.TraversedEdges,
            Is.EqualTo(
                new[]
                {
                    new Edge<Vertex>(_vertices[0], _vertices[1]),
                    new Edge<Vertex>(_vertices[1], _vertices[3]),
                    new Edge<Vertex>(_vertices[3], _vertices[5]),
                }
            )
        );
    }
}
