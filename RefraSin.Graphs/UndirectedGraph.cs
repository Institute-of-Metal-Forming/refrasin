namespace RefraSin.Graphs;

public class UndirectedGraph : IGraph
{
    private Dictionary<IVertex, IVertex[]> _adjacencies;
    private readonly IEdge[] _edges;
    private readonly IVertex[] _vertices;

    public UndirectedGraph(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges)
    {
        _vertices = vertices.ToArray();
        _edges = edges.ToArray();

        _adjacencies = _edges
            .GroupBy(e => e.Start, e => e.End)
            .Concat(
                _edges.GroupBy(e => e.End, e => e.Start)
            )
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    /// <inheritdoc />
    public int VertexCount => _vertices.Length;

    /// <inheritdoc />
    public int EdgeCount => _edges.Length;

    /// <inheritdoc />
    public IEnumerable<IVertex> Vertices => _vertices;

    /// <inheritdoc />
    public IEnumerable<IEdge> Edges => _edges;

    /// <inheritdoc />
    public IVertex Root { get; }

    /// <inheritdoc />
    public int Depth { get; }

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) => _adjacencies[vertex];

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) => _adjacencies[vertex];
}