namespace RefraSin.Graphs;

public class RootedDirectedGraph<TVertex, TEdge>
    : DirectedGraph<TVertex, TEdge>,
        IRootedGraph<TVertex, TEdge>
    where TVertex : IVertex
    where TEdge : IEdge<TVertex>
{
    public RootedDirectedGraph(
        TVertex root,
        IEnumerable<TVertex> vertices,
        IEnumerable<TEdge> edges
    )
        : base(vertices, edges)
    {
        Root = root;
    }

    /// <inheritdoc />
    public TVertex Root { get; }
}
