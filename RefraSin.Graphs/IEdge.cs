namespace RefraSin.Graphs;

public interface IEdge : IEquatable<IEdge>
{
    IVertex Start { get; }
    IVertex End { get; }

    bool IsDirected { get; }

    public bool IsEdgeFrom(IVertex from);

    public bool IsEdgeTo(IVertex to);

    public bool IsEdgeFromTo(IVertex from, IVertex to) => IsEdgeFrom(from) && IsEdgeTo(to);
}