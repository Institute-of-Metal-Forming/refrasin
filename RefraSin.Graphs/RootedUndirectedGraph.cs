namespace RefraSin.Graphs;

public class RootedUndirectedGraph : UndirectedGraph, IRootedGraph
{
    private readonly Lazy<IReadOnlyList<(IEdge, IVertex)>> _breadthFirstSearchFromRoot;
    private readonly Lazy<IReadOnlyList<(IEdge, IVertex)>> _depthFirstSearchFromRoot;

    /// <inheritdoc />
    internal RootedUndirectedGraph(IVertex root, IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges) : base(vertices, edges)
    {
        Root = new Vertex(root);

        _breadthFirstSearchFromRoot = new Lazy<IReadOnlyList<(IEdge, IVertex)>>(() => BreadthFirstSearch().ToArray());
        _depthFirstSearchFromRoot = new Lazy<IReadOnlyList<(IEdge, IVertex)>>(() => DepthFirstSearch().ToArray());
    }

    /// <inheritdoc />
    public IVertex Root { get; }

    /// <inheritdoc />
    public int Depth { get; }

    public IEnumerable<(IEdge, IVertex)> BreadthFirstSearch() =>
        _breadthFirstSearchFromRoot.IsValueCreated
            ? _breadthFirstSearchFromRoot.Value
            : ((IRootedGraph)this).BreadthFirstSearch();

    public IEnumerable<(IEdge, IVertex)> DepthFirstSearch() =>
        _depthFirstSearchFromRoot.IsValueCreated
            ? _depthFirstSearchFromRoot.Value
            : ((IRootedGraph)this).DepthFirstSearch();
}