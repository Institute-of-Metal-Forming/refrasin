namespace RefraSin.Graphs.Tests;

[TestFixture]
public class TestDepthFirstExplorer
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
            new(_vertices[0], _vertices[1]),
            new(_vertices[1], _vertices[2]),
            new(_vertices[1], _vertices[3]),
            new(_vertices[0], _vertices[4]),
            new(_vertices[2], _vertices[5]),
        ];
    }

    [Test]
    public void TestOrderDirected()
    {
        var graph = new DirectedGraph<Vertex, Edge<Vertex>>(_vertices, _edges);

        var explorer = DepthFirstExplorer<Vertex, Edge<Vertex>>.Explore(graph, _vertices[0]);

        Assert.That(
            explorer.TraversedEdges,
            Is.EqualTo(
                new IEdge[]
                {
                    new Edge<Vertex>(_vertices[0], _vertices[1]),
                    new Edge<Vertex>(_vertices[1], _vertices[2]),
                    new Edge<Vertex>(_vertices[2], _vertices[5]),
                    new Edge<Vertex>(_vertices[5], _vertices[2]),
                    new Edge<Vertex>(_vertices[1], _vertices[3]),
                    new Edge<Vertex>(_vertices[0], _vertices[4]),
                }
            )
        );
    }
}
