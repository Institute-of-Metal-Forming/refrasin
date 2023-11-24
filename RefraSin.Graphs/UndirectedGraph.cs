namespace RefraSin.Graphs;

public class UndirectedGraph<TVertex> : IGraph<TVertex, UndirectedEdge<TVertex>> where TVertex : IVertex
{
    private readonly Lazy<Dictionary<TVertex, TVertex[]>> _adjacenciesOf;

    internal UndirectedGraph(ISet<TVertex> vertices, ISet<UndirectedEdge<TVertex>> edges)
    {
        Vertices = vertices;
        Edges = edges;
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
            .GroupBy(e => e.Start, e => e.End)
            .Concat(
                Edges.GroupBy(e => e.End, e => e.Start)
            )
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