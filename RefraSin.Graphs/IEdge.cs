namespace RefraSin.Graphs;

public interface IEdge : IEquatable<IEdge>
{
    IVertex Start { get; }
    IVertex End { get; }

    bool IsDirected { get; }
}