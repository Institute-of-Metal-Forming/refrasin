namespace RefraSin.Graphs;

public class DirectedEdge<TVertex> : IEdge<TVertex> where TVertex : IVertex
{
    public DirectedEdge(IEdge<TVertex> edge) : this(edge.From, edge.To) { }

    public DirectedEdge(TVertex start, TVertex end)
    {
        From = start;
        To = end;
    }

    public void Deconstruct(out TVertex start, out TVertex end)
    {
        start = From;
        end = To;
    }

    /// <inheritdoc />
    public TVertex From { get; }

    /// <inheritdoc />
    public TVertex To { get; }

    /// <inheritdoc />
    public bool IsDirected => true;

    /// <inheritdoc />
    public bool Equals(IEdge<TVertex>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return From.Equals(other.From) && To.Equals(other.To);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as IEdge<TVertex>);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(From, To);

    /// <inheritdoc />
    public override string ToString() => $"DirectedEdge from {From} to {To}";

    public bool IsEdgeFrom(TVertex from) => From.Equals(from);

    public bool IsEdgeTo(TVertex to) => To.Equals(to);

    public DirectedEdge<TVertex> Reversed() => new(To, From);

    /// <inheritdoc />
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}