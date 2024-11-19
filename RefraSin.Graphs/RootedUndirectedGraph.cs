namespace RefraSin.Graphs;

public class RootedUndirectedGraph<TVertex>
    : UndirectedGraph<TVertex>,
        IRootedGraph<TVertex, Edge<TVertex>>
    where TVertex : IVertex
{
    public RootedUndirectedGraph(
        TVertex root,
        IEnumerable<TVertex> vertices,
        IEnumerable<IEdge<TVertex>> edges
    )
        : base(vertices, edges)
    {
        Root = root;
    }

    /// <inheritdoc />
    public TVertex Root { get; }

    /// <inheritdoc />
    public int Depth { get; }
}
