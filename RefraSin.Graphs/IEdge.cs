namespace RefraSin.Graphs;

public interface IEdge<TVertex> : IEquatable<IEdge<TVertex>> where TVertex : IVertex
{
    TVertex From { get; }
    TVertex To { get; }

    bool IsDirected { get; }

    public bool IsEdgeFrom(TVertex from);

    public bool IsEdgeTo(TVertex to);

    public bool IsEdgeAt(TVertex vertex) => IsEdgeFrom(vertex) || IsEdgeTo(vertex);

    public bool IsEdgeFromTo(TVertex from, TVertex to) => IsEdgeFrom(from) && IsEdgeTo(to);

    public IEdge<TVertex> Reversed();
}