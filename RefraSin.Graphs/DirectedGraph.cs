namespace RefraSin.Graphs;

public class DirectedGraph : IGraph
{
    private readonly Lazy<Dictionary<IVertex, IVertex[]>> _childrenOf;
    private readonly Lazy<Dictionary<IVertex, IVertex[]>> _parentsOf;
    private readonly HashSet<DirectedEdge> _edges;
    private readonly HashSet<Vertex> _vertices;

    public DirectedGraph(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges)
    {
        _vertices = vertices.Select(v => new Vertex(v)).ToHashSet();
        _edges = ConvertEdges(edges).ToHashSet();

        _childrenOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitParentsOf);
    }

    private IEnumerable<DirectedEdge> ConvertEdges(IEnumerable<IEdge> edges)
    {
        foreach (var edge in edges)
        {
            yield return new DirectedEdge(edge);
            if (!edge.IsDirected)
                yield return new DirectedEdge(edge.End, edge.Start);
        }
    }

    private Dictionary<IVertex, IVertex[]> InitChildrenOf() =>
        _edges
            .GroupBy(e => e.Start, e => e.End)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<IVertex, IVertex[]> InitParentsOf() =>
        _edges
            .GroupBy(e => e.End, e => e.Start)
            .ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public int VertexCount => _vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => _edges.Count;

    /// <inheritdoc />
    public IEnumerable<IVertex> Vertices => _vertices;

    /// <inheritdoc />
    public IEnumerable<IEdge> Edges => _edges;

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) => _childrenOf.Value[vertex];

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) => _parentsOf.Value[vertex];
}