namespace RefraSin.Graphs;

public class DirectedGraph<TVertex> : IGraph<TVertex, DirectedEdge<TVertex>> where TVertex : IVertex
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _childrenOf;
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _parentsOf;
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _adjacentsOf;

    private readonly Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>> _edgesAt;
    private readonly Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>> _edgesFrom;
    private readonly Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>> _edgesTo;

    public DirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<DirectedEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.ToHashSet();

        _childrenOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitParentsOf);
        _adjacentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacentsOf);
        _edgesAt = new Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>>(InitEdgesAt);
        _edgesFrom = new Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>>(InitEdgesFrom);
        _edgesTo = new Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>>(InitEdgesTo);
    }

    public DirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<IEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = ConvertEdges(edges).ToHashSet();

        _childrenOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitParentsOf);
        _adjacentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacentsOf);
        _edgesAt = new Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>>(InitEdgesAt);
        _edgesFrom = new Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>>(InitEdgesFrom);
        _edgesTo = new Lazy<Dictionary<TVertex, DirectedEdge<TVertex>[]>>(InitEdgesTo);
    }

    public static DirectedGraph<TVertex> FromGraph<TEdge>(IGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex> =>
        new(graph.Vertices, (IEnumerable<IEdge<TVertex>>)graph.Edges);

    public static DirectedGraph<TVertex> FromGraphSearch(IGraphTraversal<TVertex> graphTraversal)
    {
        var edges = graphTraversal.TraversedEdges.ToArray();
        var vertices = edges.Select(e => e.To).Prepend(graphTraversal.Start);
        return new DirectedGraph<TVertex>(vertices, edges);
    }

    private IEnumerable<DirectedEdge<TVertex>> ConvertEdges(IEnumerable<IEdge<TVertex>> edges)
    {
        foreach (var edge in edges)
        {
            yield return new DirectedEdge<TVertex>(edge);
            if (!edge.IsDirected)
                yield return new DirectedEdge<TVertex>(edge.To, edge.From);
        }
    }

    private Dictionary<TVertex, TVertex[]> InitChildrenOf() =>
        Edges
            .GroupBy(e => e.From, e => e.To)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TVertex[]> InitParentsOf() =>
        Edges
            .GroupBy(e => e.To, e => e.From)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TVertex[]> InitAdjacentsOf() =>
        _childrenOf.Value.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Concat(
                _parentsOf.Value.GetValueOrDefault(kvp.Key, Array.Empty<TVertex>())
            ).ToArray()
        );

    private Dictionary<TVertex, DirectedEdge<TVertex>[]> InitEdgesFrom() =>
        Edges
            .GroupBy(e => e.From)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, DirectedEdge<TVertex>[]> InitEdgesTo() =>
        Edges
            .GroupBy(e => e.To)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, DirectedEdge<TVertex>[]> InitEdgesAt() =>
        _edgesFrom.Value.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Concat(
                _edgesTo.Value.GetValueOrDefault(kvp.Key, Array.Empty<DirectedEdge<TVertex>>())
            ).ToArray()
        );

    /// <inheritdoc />
    public int VertexCount => Vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => Edges.Count;

    /// <inheritdoc />
    public ISet<TVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<DirectedEdge<TVertex>> Edges { get; }

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) => _childrenOf.Value.GetValueOrDefault(vertex, Array.Empty<TVertex>());

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) => _parentsOf.Value.GetValueOrDefault(vertex, Array.Empty<TVertex>());

    public IEnumerable<TVertex> AdjacentsOf(TVertex vertex) => _adjacentsOf.Value.GetValueOrDefault(vertex, Array.Empty<TVertex>());

    public IEnumerable<DirectedEdge<TVertex>> EdgesAt(TVertex vertex) =>
        _edgesAt.Value.GetValueOrDefault(vertex, Array.Empty<DirectedEdge<TVertex>>());

    public IEnumerable<DirectedEdge<TVertex>> EdgesFrom(TVertex vertex) =>
        _edgesFrom.Value.GetValueOrDefault(vertex, Array.Empty<DirectedEdge<TVertex>>());

    public IEnumerable<DirectedEdge<TVertex>> EdgesTo(TVertex vertex) =>
        _edgesTo.Value.GetValueOrDefault(vertex, Array.Empty<DirectedEdge<TVertex>>());

    public DirectedGraph<TVertex> Reversed()
    {
        var reversedEdges = Edges.Select(e => new DirectedEdge<TVertex>(e.To, e.From)).ToHashSet();

        return new DirectedGraph<TVertex>(Vertices, reversedEdges);
    }
}