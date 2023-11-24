namespace RefraSin.Graphs;

public class UndirectedGraph : IGraph
{
    private readonly Lazy<Dictionary<IVertex, IVertex[]>> _adjacenciesOf;

    internal UndirectedGraph(ISet<IVertex> vertices, ISet<IEdge> edges)
    {
        Vertices = vertices;
        Edges = edges;
        _adjacenciesOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitAdjacenciesOf);
    }

    public UndirectedGraph(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges)
    {
        Vertices = vertices.Select(v => (IVertex)new Vertex(v)).ToHashSet();
        Edges = edges.Select(e => (IEdge)new UndirectedEdge(e)).ToHashSet();
        _adjacenciesOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitAdjacenciesOf);
    }

    private Dictionary<IVertex, IVertex[]> InitAdjacenciesOf() =>
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
    public ISet<IVertex> Vertices { get; }

    /// <inheritdoc />
    public ISet<IEdge> Edges { get; }

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) => _adjacenciesOf.Value[vertex];

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) => _adjacenciesOf.Value[vertex];

    /// <inheritdoc />
    public IRootedGraph RootTo(IVertex vertex) => new RootedUndirectedGraph(vertex, Vertices, Edges);
}