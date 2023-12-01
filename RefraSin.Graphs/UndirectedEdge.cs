namespace RefraSin.Graphs;

public class UndirectedEdge<TVertex> : IEdge<TVertex> where TVertex : IVertex
{
    public UndirectedEdge(IEdge<TVertex> edge) : this(edge.Start, edge.End) { }

    public UndirectedEdge(TVertex start, TVertex end)
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
    public bool IsDirected => false;

    /// <inheritdoc />
    public bool Equals(IEdge<TVertex>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (Start.Equals(other.Start) && End.Equals(other.End)) || (Start.Equals(other.End) && End.Equals(other.Start));
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => Equals(obj as IEdge<TVertex>);

    /// <inheritdoc />
    public override int GetHashCode() => Start.GetHashCode() * End.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => $"UndirectedEdge from {Start} to {End}";

    /// <inheritdoc />
    public bool IsEdgeAt(TVertex vertex) => Start.Equals(vertex) || End.Equals(vertex);

    public bool IsEdgeFrom(TVertex from) => IsEdgeAt(from);

    public bool IsEdgeTo(TVertex to) => IsEdgeAt(to);

    public UndirectedEdge<TVertex> Reversed() => new(End, Start);

    /// <inheritdoc />
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}