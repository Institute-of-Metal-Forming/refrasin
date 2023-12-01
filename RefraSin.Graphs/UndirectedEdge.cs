namespace RefraSin.Graphs;

public record UndirectedEdge<TVertex>(TVertex Start, TVertex End) : IEdge<TVertex> where TVertex : IVertex
{
    public UndirectedEdge(IEdge<TVertex> edge) : this(edge.Start, edge.End) { }

    /// <inheritdoc />
    public bool Equals(IEdge<TVertex>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (Start.Equals(other.Start) && End.Equals(other.End)) || (Start.Equals(other.End) && End.Equals(other.Start));
    }

    /// <inheritdoc />
    public virtual bool Equals(UndirectedEdge<TVertex>? other) => Equals((IEdge<TVertex>?)other);

    /// <inheritdoc />
    public override int GetHashCode() => Start.GetHashCode() * End.GetHashCode();

    /// <inheritdoc />
    public bool IsDirected => false;

    public bool IsEdgeAt(TVertex vertex) => Start.Equals(vertex) || End.Equals(vertex);

    public bool IsEdgeFrom(TVertex from) => IsEdgeAt(from);

    public bool IsEdgeTo(TVertex to) => IsEdgeAt(to);

    public UndirectedEdge<TVertex> Reversed() => new(End, Start);

    /// <inheritdoc />
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}