namespace RefraSin.Graphs;

public sealed record UndirectedEdge(IVertex Start, IVertex End) : IEdge
{
    public UndirectedEdge(IEdge edge) : this(edge.Start, edge.End) { }

    /// <inheritdoc />
    public bool Equals(IEdge? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (Start.Equals(other.Start) && End.Equals(other.End)) || (Start.Equals(other.End) && End.Equals(other.Start));
    }

    /// <inheritdoc />
    public override int GetHashCode() => Start.GetHashCode() * End.GetHashCode();

    /// <inheritdoc />
    public bool IsDirected => false;

    public bool IsEdgeFrom(IVertex from) => Start == from || End == from;

    public bool IsEdgeTo(IVertex to) => Start == to || End == to;
}