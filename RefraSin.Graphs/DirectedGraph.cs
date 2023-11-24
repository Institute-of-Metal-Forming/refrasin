namespace RefraSin.Graphs;

public class DirectedGraph : IGraph
{
    private readonly Lazy<Dictionary<IVertex, IVertex[]>> _childrenOf;
    private readonly Lazy<Dictionary<IVertex, IVertex[]>> _parentsOf;

    internal DirectedGraph(ISet<IVertex> vertices, ISet<IEdge> edges)
    {
        Vertices = vertices;
        Edges = edges;

        _childrenOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitParentsOf);
    }

    public DirectedGraph(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges)
    {
        Vertices = vertices.Select(v => (IVertex)new Vertex(v)).ToHashSet();
        Edges = ConvertEdges(edges).ToHashSet();

        _childrenOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitParentsOf);
    }

    private IEnumerable<IEdge> ConvertEdges(IEnumerable<IEdge> edges)
    {
        foreach (var edge in edges)
        {
            yield return new DirectedEdge(edge);
            if (!edge.IsDirected)
                yield return new DirectedEdge(edge.End, edge.Start);
        }
    }

    private Dictionary<IVertex, IVertex[]> InitChildrenOf() =>
        Edges
            .GroupBy(e => e.Start, e => e.End)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<IVertex, IVertex[]> InitParentsOf() =>
        Edges
            .GroupBy(e => e.End, e => e.Start)
            .ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public int VertexCount => Vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => Edges.Count;

    /// <inheritdoc />
    public ISet<IVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<IEdge> Edges { get; }

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) => _childrenOf.Value[vertex];

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) => _parentsOf.Value[vertex];

    /// <inheritdoc />
    public IRootedGraph RootTo(IVertex vertex) => new RootedDirectedGraph(vertex, Vertices, Edges);
}