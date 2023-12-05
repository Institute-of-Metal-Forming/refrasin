namespace RefraSin.Graphs;

public class UndirectedGraph<TVertex> : IGraph<TVertex, UndirectedEdge<TVertex>> where TVertex : IVertex
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _adjacentsOf;
    private readonly Lazy<Dictionary<TVertex, UndirectedEdge<TVertex>[]>> _edgesAt;

    public UndirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<UndirectedEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.ToHashSet();
        _adjacentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacentsOf);
        _edgesAt = new Lazy<Dictionary<TVertex, UndirectedEdge<TVertex>[]>>(InitEdgesAt);
    }

    public UndirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<IEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.Select(e => new UndirectedEdge<TVertex>(e)).ToHashSet();
        _adjacentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacentsOf);
        _edgesAt = new Lazy<Dictionary<TVertex, UndirectedEdge<TVertex>[]>>(InitEdgesAt);
    }

    public static UndirectedGraph<TVertex> FromGraph<TEdge>(IGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex> =>
        new(graph.Vertices, (IEnumerable<IEdge<TVertex>>)graph.Edges);

    public static UndirectedGraph<TVertex> FromGraphSearch(IGraphTraversal<TVertex> graphTraversal)
    {
        var edges = graphTraversal.TraversedEdges.ToArray();
        var vertices = edges.Select(e => e.End).Prepend(graphTraversal.Start);
        return new UndirectedGraph<TVertex>(vertices, edges);
    }

    private Dictionary<TVertex, TVertex[]> InitAdjacentsOf() =>
        Edges
            .Concat(Edges.Select(e => e.Reversed()))
            .GroupBy(e => e.Start, e => e.End)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, UndirectedEdge<TVertex>[]> InitEdgesAt() =>
        Edges
            .Concat(Edges.Select(e => e.Reversed()))
            .GroupBy(e => e.Start)
            .ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public int VertexCount => Vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => Edges.Count;

    /// <inheritdoc />
    public ISet<TVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<UndirectedEdge<TVertex>> Edges { get; }

    public IEnumerable<UndirectedEdge<TVertex>> EdgesTo(TVertex vertex) => EdgesAt(vertex);

    public IEnumerable<UndirectedEdge<TVertex>> EdgesFrom(TVertex vertex) => EdgesAt(vertex);

    public IEnumerable<UndirectedEdge<TVertex>> EdgesAt(TVertex vertex) =>
        _edgesAt.Value.GetValueOrDefault(vertex, Array.Empty<UndirectedEdge<TVertex>>());

    public IEnumerable<TVertex> AdjacentsOf(TVertex vertex) => _adjacentsOf.Value.GetValueOrDefault(vertex, Array.Empty<TVertex>());

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) => AdjacentsOf(vertex);

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) => AdjacentsOf(vertex);
}