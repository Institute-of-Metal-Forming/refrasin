namespace RefraSin.Graphs;

public interface IEdge<TVertex> : IEquatable<IEdge<TVertex>> where TVertex : IVertex
{
    TVertex Start { get; }
    TVertex End { get; }

    bool IsDirected { get; }

    public bool IsEdgeFrom(TVertex from);

    public bool IsEdgeTo(TVertex to);

    public bool IsEdgeFromTo(TVertex from, TVertex to) => IsEdgeFrom(from) && IsEdgeTo(to);
}