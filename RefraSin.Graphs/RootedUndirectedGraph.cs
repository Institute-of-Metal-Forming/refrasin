namespace RefraSin.Graphs;

public class RootedUndirectedGraph : UndirectedGraph, IRootedGraph
{
    internal RootedUndirectedGraph(IVertex root, ISet<IVertex> vertices, ISet<IEdge> edges) : base(vertices, edges)
    {
        Root = new Vertex(root);
    }

    public RootedUndirectedGraph(IVertex root, IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges) : base(vertices, edges)
    {
        Root = new Vertex(root);
    }

    /// <inheritdoc />
    public IVertex Root { get; }

    /// <inheritdoc />
    public int Depth { get; }
}