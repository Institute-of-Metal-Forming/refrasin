namespace RefraSin.Graphs;

public class UndirectedGraph<TVertex> : IGraph<TVertex, UndirectedEdge<TVertex>> where TVertex : IVertex
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _adjacenciesOf;

    public UndirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<UndirectedEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.ToHashSet();
        _adjacenciesOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacenciesOf);
    }

    public UndirectedGraph(IEnumerable<TVertex> vertices, IEnumerable<IEdge<TVertex>> edges)
    {
        Vertices = vertices.ToHashSet();
        Edges = edges.Select(e => new UndirectedEdge<TVertex>(e)).ToHashSet();
        _adjacenciesOf = new Lazy<Dictionary<TVertex, TVertex[]>>(InitAdjacenciesOf);
    }

    private Dictionary<TVertex, TVertex[]> InitAdjacenciesOf() =>
        Edges
            .Concat(Edges.Select(e => e.Reversed()))
            .GroupBy(e => e.Start, e => e.End)
            .ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public int VertexCount => Vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => Edges.Count;

    /// <inheritdoc />
    public ISet<TVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<UndirectedEdge<TVertex>> Edges { get; }

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) => _adjacenciesOf.Value[vertex];

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) => _adjacenciesOf.Value[vertex];
}