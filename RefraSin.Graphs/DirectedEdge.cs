namespace RefraSin.Graphs;

public record DirectedEdge<TVertex>(TVertex Start, TVertex End) : IEdge<TVertex> where TVertex : IVertex
{
    public DirectedEdge(IEdge<TVertex> edge) : this(edge.Start, edge.End) { }

    /// <inheritdoc />
    public bool Equals(IEdge<TVertex>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    /// <inheritdoc />
    public virtual bool Equals(DirectedEdge<TVertex>? other) => Equals((IEdge<TVertex>?)other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Start, End);

    /// <inheritdoc />
    public bool IsDirected => true;

    public bool IsEdgeFrom(TVertex from) => Start.Equals(from);

    public bool IsEdgeTo(TVertex to) => End.Equals(to);

    public DirectedEdge<TVertex> Reversed() => new(End, Start);

    /// <inheritdoc />
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}