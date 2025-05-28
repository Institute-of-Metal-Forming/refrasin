using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Collections;

public static class CollectionExtensions
{
    /// <summary>
    /// Creates a new <see cref="ReadOnlyParticleSurface{TContact}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyParticleSurface<TNode> ToReadOnlyParticleSurface<TNode>(
        this IEnumerable<TNode> source
    )
        where TNode : INode => new(source);

    /// <summary>
    /// Creates a new <see cref="ReadOnlyVertexCollection{TVertex}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyVertexCollection<TVertex> ToReadOnlyVertexCollection<TVertex>(
        this IEnumerable<TVertex> self
    )
        where TVertex : IVertex => new(self);
}
