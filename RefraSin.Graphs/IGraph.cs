using MoreLinq.Extensions;

namespace RefraSin.Graphs;

public interface IGraph<TVertex, TEdge> where TVertex : IVertex where TEdge : IEdge<TVertex>
{
    ISet<TVertex> Vertices { get; }

    ISet<TEdge> Edges { get; }

    int VertexCount => Vertices.Count;

    int EdgeCount => Edges.Count;

    public IEnumerable<TVertex> ParentsOf(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeTo(vertex)).Select(e => e.Start.Equals(vertex) ? e.End : e.Start);

    public IEnumerable<TVertex> ChildrenOf(TVertex vertex) =>
        Edges.Where(e => e.IsEdgeFrom(vertex)).Select(e => e.Start.Equals(vertex) ? e.End : e.Start);
}

public interface IRootedGraph<TVertex, TEdge> : IGraph<TVertex, TEdge> where TVertex : IVertex where TEdge : IEdge<TVertex>
{
    TVertex Root { get; }

    int Depth { get; }
}