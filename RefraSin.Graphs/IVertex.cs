namespace RefraSin.Graphs;

public interface IVertex : IEquatable<IVertex>
{
    Guid Id { get; }

    /// <inheritdoc />
    bool IEquatable<IVertex>.Equals(IVertex? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Id.Equals(other.Id);
    }
}
