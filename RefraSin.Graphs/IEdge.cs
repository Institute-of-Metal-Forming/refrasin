namespace RefraSin.Graphs;

public interface IEdge<out TVertex> : IEdge
    where TVertex : IVertex
{
    new TVertex From { get; }
    new TVertex To { get; }

    Guid IEdge.From => From.Id;
    Guid IEdge.To => To.Id;
}

public interface IEdge : IEquatable<IEdge>
{
    Guid From { get; }
    Guid To { get; }

    bool IEquatable<IEdge>.Equals(IEdge? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (From == other.From && To == other.To)
            return true;

        return false;
    }
}
