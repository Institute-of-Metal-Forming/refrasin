namespace RefraSin.Graphs;

public record Vertex(Guid Id, string Label = "") : IVertex
{
    public Vertex(IVertex vertex): this(vertex.Id)
    {
        if (vertex is Vertex v)
        {
            Label = v.Label;
        }
    }

    /// <inheritdoc />
    public virtual bool Equals(IVertex? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();
}