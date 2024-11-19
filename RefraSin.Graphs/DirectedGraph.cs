namespace RefraSin.Graphs;

public class DirectedGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
    where TVertex : IVertex
    where TEdge : IEdge<TVertex>
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _childrenOf;
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _parentsOf;
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _adjacentsOf;

    private readonly Lazy<Dictionary<TVertex, TEdge[]>> _edgesAt;
    private readonly Lazy<Dictionary<TVertex, TEdge[]>> _edgesFrom;
    private readonly Lazy<Dictionary<TVertex, TEdge[]>> _edgesTo;

    public DirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<TEdge> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.ToHashSet();

        _childrenOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitParentsOf);
        _adjacentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacentsOf);
        _edgesAt = new Lazy<Dictionary<TVertex, TEdge[]>>(InitEdgesAt);
        _edgesFrom = new Lazy<Dictionary<TVertex, TEdge[]>>(InitEdgesFrom);
        _edgesTo = new Lazy<Dictionary<TVertex, TEdge[]>>(InitEdgesTo);
    }

    public static DirectedGraph<TVertex, TEdge> FromGraph(IGraph<TVertex, TEdge> graph) =>
        new(graph.Vertices, graph.Edges);

    public static DirectedGraph<TVertex, TEdge> FromGraphSearch(
        IGraphTraversal<TVertex, TEdge> graphTraversal
    )
    {
        var edges = graphTraversal.TraversedEdges.ToArray();
        var vertices = edges.Select(e => e.To).Prepend(graphTraversal.Start);
        return new DirectedGraph<TVertex, TEdge>(vertices, edges);
    }

    private Dictionary<TVertex, TVertex[]> InitChildrenOf() =>
        Edges.GroupBy(e => e.From, e => e.To).ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TVertex[]> InitParentsOf() =>
        Edges.GroupBy(e => e.To, e => e.From).ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TVertex[]> InitAdjacentsOf() =>
        _childrenOf.Value.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Concat(_parentsOf.Value.GetValueOrDefault(kvp.Key, [])).ToArray()
        );

    private Dictionary<TVertex, TEdge[]> InitEdgesFrom() =>
        Edges.GroupBy(e => e.From).ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TEdge[]> InitEdgesTo() =>
        Edges.GroupBy(e => e.To).ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TEdge[]> InitEdgesAt() =>
        _edgesFrom.Value.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Concat(_edgesTo.Value.GetValueOrDefault(kvp.Key, [])).ToArray()
        );

    /// <inheritdoc />
    public int VertexCount => Vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => Edges.Count;

    /// <inheritdoc />
    public ISet<TVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<TEdge> Edges { get; }

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) =>
        _childrenOf.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) =>
        _parentsOf.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TVertex> AdjacentsOf(TVertex vertex) =>
        _adjacentsOf.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TEdge> EdgesAt(TVertex vertex) =>
        _edgesAt.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TEdge> EdgesFrom(TVertex vertex) =>
        _edgesFrom.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TEdge> EdgesTo(TVertex vertex) =>
        _edgesTo.Value.GetValueOrDefault(vertex, []);
}
