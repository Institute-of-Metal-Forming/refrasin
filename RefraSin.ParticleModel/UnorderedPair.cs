namespace RefraSin.ParticleModel;

public readonly struct UnorderedPair<TVertex>(TVertex first, TVertex second)
    : IEquatable<UnorderedPair<TVertex>>
    where TVertex : IVertex
{
    public readonly TVertex First = first;

    public readonly TVertex Second = second;

    public bool Equals(UnorderedPair<TVertex> other)
    {
        if (First.Equals(other.First) && Second.Equals(other.Second))
            return true;
        if (First.Equals(other.Second) && Second.Equals(other.First))
            return true;
        return false;
    }

    public override bool Equals(object? obj) => obj is UnorderedPair<TVertex> pair && Equals(pair);

    public override int GetHashCode() => HashCode.Combine(First, Second);

    public static bool operator ==(UnorderedPair<TVertex> left, UnorderedPair<TVertex> right) =>
        left.Equals(right);

    public static bool operator !=(UnorderedPair<TVertex> left, UnorderedPair<TVertex> right) =>
        !(left == right);
}
