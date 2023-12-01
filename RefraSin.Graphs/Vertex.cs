namespace RefraSin.Graphs;

public class Vertex : IVertex
{
    public Vertex(IVertex vertex) : this(vertex.Id)
    {
        if (vertex is Vertex v)
        {
            Label = v.Label;
        }
    }

    public Vertex(Guid id, string label = "")
    {
        Id = id;
        Label = label;
    }

    public Guid Id { get; }
    public string Label { get; }

    /// <inheritdoc />
    public virtual bool Equals(IVertex? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => Equals(obj as IVertex);

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => string.IsNullOrWhiteSpace(Label) ? $"Vertex {Id}" : $"Vertex {Label} ({Id})";
}