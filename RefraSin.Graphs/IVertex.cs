namespace RefraSin.Graphs;

public interface IVertex : IEquatable<IVertex>
{
    Guid Id { get; }
}