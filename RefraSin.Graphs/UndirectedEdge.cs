namespace RefraSin.Graphs;

public class UndirectedEdge<TVertex> : IEdge<TVertex> where TVertex : IVertex
{
    public UndirectedEdge(IEdge<TVertex> edge) : this(edge.From, edge.To) { }

    public UndirectedEdge(TVertex start, TVertex end)
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
    public bool IsDirected => false;

    /// <inheritdoc />
    public bool Equals(IEdge<TVertex>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (From.Equals(other.From) && To.Equals(other.To)) || (From.Equals(other.To) && To.Equals(other.From));
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => Equals(obj as IEdge<TVertex>);

    /// <inheritdoc />
    public override int GetHashCode() => From.GetHashCode() * To.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => $"UndirectedEdge from {From} to {To}";

    /// <inheritdoc />
    public bool IsEdgeAt(TVertex vertex) => From.Equals(vertex) || To.Equals(vertex);

    public bool IsEdgeFrom(TVertex from) => IsEdgeAt(from);

    public bool IsEdgeTo(TVertex to) => IsEdgeAt(to);

    public UndirectedEdge<TVertex> Reversed() => new(To, From);

    /// <inheritdoc />
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}