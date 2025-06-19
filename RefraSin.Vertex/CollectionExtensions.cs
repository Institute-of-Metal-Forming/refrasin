namespace RefraSin.Vertex;

public static class CollectionExtensions
{
    /// <summary>
    /// Creates a new <see cref="ReadOnlyVertexCollection{TVertex}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyVertexCollection<TVertex> ToReadOnlyVertexCollection<TVertex>(
        this IEnumerable<TVertex> self
    )
        where TVertex : IVertex => new(self);
}
