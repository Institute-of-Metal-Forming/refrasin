namespace RefraSin.Graphs;

public class DirectedGraph<TVertex> : IGraph<TVertex, DirectedEdge<TVertex>> where TVertex : IVertex
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _childrenOf;
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _parentsOf;

    public DirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<DirectedEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.ToHashSet();

        _childrenOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitParentsOf);
    }

    public DirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<IEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = ConvertEdges(edges).ToHashSet();

        _childrenOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitChildrenOf);
        _parentsOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitParentsOf);
    }

    public static DirectedGraph<TVertex> FromGraph<TEdge>(IGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex> =>
        new(graph.Vertices, (IEnumerable<IEdge<TVertex>>)graph.Edges);

    public static DirectedGraph<TVertex> FromGraphSearch(IGraphSearch<TVertex> graphSearch)
    {
        var edges = graphSearch.ExploredEdges.ToArray();
        var vertices = edges.Select(e => e.End).Prepend(graphSearch.Start);
        return new DirectedGraph<TVertex>(vertices, edges);
    }

    private IEnumerable<DirectedEdge<TVertex>> ConvertEdges(IEnumerable<IEdge<TVertex>> edges)
    {
        foreach (var edge in edges)
        {
            yield return new DirectedEdge<TVertex>(edge);
            if (!edge.IsDirected)
                yield return new DirectedEdge<TVertex>(edge.End, edge.Start);
        }
    }

    private Dictionary<TVertex, TVertex[]> InitChildrenOf() =>
        Edges
            .GroupBy(e => e.Start, e => e.End)
            .ToDictionary(g => g.Key, g => g.ToArray());

    private Dictionary<TVertex, TVertex[]> InitParentsOf() =>
        Edges
            .GroupBy(e => e.End, e => e.Start)
            .ToDictionary(g => g.Key, g => g.ToArray());

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

    public DirectedGraph<TVertex> Reversed()
    {
        var reversedEdges = Edges.Select(e => new DirectedEdge<TVertex>(e.End, e.Start)).ToHashSet();

        return new DirectedGraph<TVertex>(Vertices, reversedEdges);
    }
}