namespace RefraSin.Graphs;

public interface IEdge<TVertex> : IEdge, IEquatable<IEdge<TVertex>> where TVertex : IVertex
{
    new TVertex From { get; }
    new TVertex To { get; }

    Guid IEdge.From => From.Id;
    Guid IEdge.To => To.Id;

    new IEdge<TVertex> Reversed();
    bool IEquatable<IEdge<TVertex>>.Equals(IEdge<TVertex>? other) => ((IEdge)this).Equals(other);
}

public interface IEdge : IEquatable<IEdge>
{
    Guid From { get; }
    Guid To { get; }

    bool IsDirected { get; }

    IEdge Reversed();

    bool IEquatable<IEdge>.Equals(IEdge? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (From == other.From && To == other.To && IsDirected == other.IsDirected)
            return true;

        if (IsDirected == false)
            if (To == other.From && From == other.To && IsDirected == other.IsDirected)
                return true;

        return false;
    }
}