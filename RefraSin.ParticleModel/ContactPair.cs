using RefraSin.Vertex;

namespace RefraSin.ParticleModel;

public readonly struct ContactPair<TVertex>(Guid id, TVertex first, TVertex second)
    : IEquatable<ContactPair<TVertex>>,
        IVertex
    where TVertex : IVertex
{
    public readonly Guid Id = id;
    Guid IVertex.Id => Id;

    public readonly TVertex First = first;

    public readonly TVertex Second = second;

    public bool Equals(ContactPair<TVertex> other)
    {
        if (First.Equals(other.First) && Second.Equals(other.Second))
            return true;
        if (First.Equals(other.Second) && Second.Equals(other.First))
            return true;
        return false;
    }

    public override bool Equals(object? obj) => obj is ContactPair<TVertex> pair && Equals(pair);

    public override int GetHashCode() => HashCode.Combine(First, Second);

    public static bool operator ==(ContactPair<TVertex> left, ContactPair<TVertex> right) =>
        left.Equals(right);

    public static bool operator !=(ContactPair<TVertex> left, ContactPair<TVertex> right) =>
        !(left == right);
}
