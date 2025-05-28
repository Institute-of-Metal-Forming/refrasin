using RefraSin.ParticleModel.Nodes;

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
}
