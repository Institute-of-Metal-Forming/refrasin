namespace RefraSin.Graphs;

public class UndirectedGraph : IGraph
{
    private readonly Lazy<Dictionary<IVertex, IVertex[]>> _adjacenciesOf;
    private readonly HashSet<UndirectedEdge> _edges;
    private readonly HashSet<Vertex> _vertices;

    public UndirectedGraph(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges)
    {
        _vertices = vertices.Select(v => new Vertex(v)).ToHashSet();
        _edges = edges.Select(e => new UndirectedEdge(e)).ToHashSet();

        _adjacenciesOf = new Lazy<Dictionary<IVertex, IVertex[]>>(InitAdjacenciesOf);
    }

    private Dictionary<IVertex, IVertex[]> InitAdjacenciesOf() =>
        _edges
            .GroupBy(e => e.Start, e => e.End)
            .Concat(
                _edges.GroupBy(e => e.End, e => e.Start)
            )
            .ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public int VertexCount => _vertices.Count;

    /// <inheritdoc />
    public int EdgeCount => _edges.Count;

    /// <inheritdoc />
    public IEnumerable<IVertex> Vertices => _vertices;

    /// <inheritdoc />
    public IEnumerable<IEdge> Edges => _edges;

    /// <inheritdoc />
    public IVertex Root { get; }

    /// <inheritdoc />
    public int Depth { get; }

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) => _adjacenciesOf.Value[vertex];

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) => _adjacenciesOf.Value[vertex];
}