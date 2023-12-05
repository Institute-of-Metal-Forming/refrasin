namespace RefraSin.Graphs;

public class DirectedEdge<TVertex> : IEdge<TVertex> where TVertex : IVertex
{
    public DirectedEdge(IEdge<TVertex> edge) : this(edge.Start, edge.End) { }

    public DirectedEdge(TVertex start, TVertex end)
    {
        Start = start;
        End = end;
    }

    public void Deconstruct(out TVertex start, out TVertex end)
    {
        start = Start;
        end = End;
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public TVertex End { get; }

    /// <inheritdoc />
    public bool IsDirected => true;

    /// <inheritdoc />
    public bool Equals(IEdge<TVertex>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => Equals(obj as IEdge<TVertex>);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Start, End);

    /// <inheritdoc />
    public override string ToString() => $"DirectedEdge from {Start} to {End}";

    public bool IsEdgeFrom(TVertex from) => Start.Equals(from);

    public bool IsEdgeTo(TVertex to) => End.Equals(to);

    public DirectedEdge<TVertex> Reversed() => new(End, Start);

    /// <inheritdoc />
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}