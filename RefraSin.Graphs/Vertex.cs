namespace RefraSin.Graphs;

public record Vertex(Guid Id, string Label) : IVertex
{
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