namespace RefraSin.Graphs;

public class UndirectedGraph<TVertex> : IGraph<TVertex, Edge<TVertex>>
    where TVertex : IVertex
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _adjacentsOf;
    private readonly Lazy<Dictionary<TVertex, Edge<TVertex>[]>> _edgesAt;

    public UndirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<IEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.Select(e => new Edge<TVertex>(e.From, e.To, false)).ToHashSet();
        _adjacentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacentsOf);
        _edgesAt = new Lazy<Dictionary<TVertex, Edge<TVertex>[]>>(InitEdgesAt);
    }

    public static UndirectedGraph<TVertex> FromGraph<TEdge>(IGraph<TVertex, TEdge> graph)
        where TEdge : IEdge<TVertex> =>
        new(graph.Vertices, (IEnumerable<IEdge<TVertex>>)graph.Edges);

    public static UndirectedGraph<TVertex> FromGraphSearch(IGraphTraversal<TVertex> graphTraversal)
    {
        var edges = graphTraversal.TraversedEdges.ToArray();
        var vertices = edges.Select(e => e.To).Prepend(graphTraversal.Start);
        return new UndirectedGraph<TVertex>(vertices, edges);
    }

    private Dictionary<TVertex, TVertex[]> InitAdjacentsOf() =>
        Edges
            .Concat(Edges.Select(e => e.Reversed()))
            .GroupBy(e => e.From, e => e.To)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, Edge<TVertex>[]> InitEdgesAt() =>
        Edges
            .Concat(Edges.Select(e => e.Reversed()))
            .GroupBy(e => e.From)
            .ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public int VertexCount => Vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => Edges.Count;

    /// <inheritdoc />
    public ISet<TVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<Edge<TVertex>> Edges { get; }

    public IEnumerable<Edge<TVertex>> EdgesTo(TVertex vertex) => EdgesAt(vertex);

    public IEnumerable<Edge<TVertex>> EdgesFrom(TVertex vertex) => EdgesAt(vertex);

    public IEnumerable<Edge<TVertex>> EdgesAt(TVertex vertex) =>
        _edgesAt.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TVertex> AdjacentsOf(TVertex vertex) =>
        _adjacentsOf.Value.GetValueOrDefault(vertex, []);

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) => AdjacentsOf(vertex);

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) => AdjacentsOf(vertex);
}
