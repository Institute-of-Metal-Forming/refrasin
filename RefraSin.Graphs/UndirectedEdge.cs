namespace RefraSin.Graphs;

public sealed record UndirectedEdge<TVertex>(TVertex Start, TVertex End) : IEdge<TVertex> where TVertex : IVertex
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
    public bool Equals(UndirectedEdge<TVertex>? other) => Equals((IEdge<TVertex>?)other);

    /// <inheritdoc />
    public override int GetHashCode() => Start.GetHashCode() * End.GetHashCode();

    /// <inheritdoc />
    public bool IsDirected => false;

    public bool IsEdgeFrom(TVertex from) => Start.Equals(from) || End.Equals(from);

    public bool IsEdgeTo(TVertex to) => IsEdgeFrom(to);

    /// <inheritdoc />
    public IEdge<TVertex> Reversed() => new UndirectedEdge<TVertex>(End, Start);
}