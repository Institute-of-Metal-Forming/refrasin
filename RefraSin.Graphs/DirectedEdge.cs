namespace RefraSin.Graphs;

public class DirectedEdge<TVertex>(Guid id, TVertex from, TVertex to) : IEdge<TVertex>
    where TVertex : IVertex
{
    public DirectedEdge(IEdge<TVertex> edge) : this(edge.Id, edge.From, edge.To) { }

    public DirectedEdge(TVertex from, TVertex to) : this(Helper.MergeGuids(from.Id, to.Id), from, to) { }

    public void Deconstruct(out TVertex start, out TVertex end)
    {
        start = From;
        end = To;
    }

    /// <inheritdoc />
    public Guid Id { get; } = id;

    /// <inheritdoc />
    public TVertex From { get; } = from;

    /// <inheritdoc />
    public TVertex To { get; } = to;

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