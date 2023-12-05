namespace RefraSin.Graphs;

public interface IGraph<TVertex, TEdge> where TVertex : IVertex where TEdge : IEdge<TVertex>
{
    ISet<TVertex> Vertices { get; }

    ISet<TEdge> Edges { get; }

    int VertexCount => Vertices.Count;

    int EdgeCount => Edges.Count;

    public IEnumerable<TEdge> EdgesTo(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeTo(vertex));

    public IEnumerable<TEdge> EdgesFrom(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeFrom(vertex));

    public IEnumerable<TEdge> EdgesAt(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeAt(vertex));

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeTo(vertex)).Select(e => e.From.Equals(vertex) ? e.To : e.From);

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeFrom(vertex)).Select(e => e.From.Equals(vertex) ? e.To : e.From);

    public IEnumerable<TVertex> AdjacentsOf(TVertex vertex)
    {
        var parents = ParentsOf(vertex);
        var children = ChildrenOf(vertex);

        var result = parents.ToHashSet();
        result.UnionWith(children);

        return result;
    }
}

public interface IRootedGraph<TVertex, TEdge> : IGraph<TVertex, TEdge> where TVertex : IVertex where TEdge : IEdge<TVertex>
{
    TVertex Root { get; }

    int Depth { get; }
}