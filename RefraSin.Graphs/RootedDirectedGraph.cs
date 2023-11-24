namespace RefraSin.Graphs;

public class RootedDirectedGraph : DirectedGraph, IRootedGraph
{
    internal RootedDirectedGraph(IVertex root, ISet<IVertex> vertices, ISet<IEdge> edges) : base(vertices, edges)
    {
        Root = new Vertex(root);
    }

    public RootedDirectedGraph(IVertex root, IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges) : base(vertices, edges)
    {
        Root = new Vertex(root);
    }

    /// <inheritdoc />
    public IVertex Root { get; }

    /// <inheritdoc />
    public int Depth { get; }
}