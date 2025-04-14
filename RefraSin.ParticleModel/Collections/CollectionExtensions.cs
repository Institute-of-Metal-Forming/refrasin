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
    /// Creates a new <see cref="ReadOnlyNodeCollection{TNode}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyNodeCollection<TNode> ToReadOnlyNodeCollection<TNode>(
        this IEnumerable<TNode> self
    )
        where TNode : INode => new(self);

    /// <summary>
    /// Creates a new <see cref="ReadOnlyParticleCollection{TParticle, TContact}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyParticleCollection<TParticle, TNode> ToReadOnlyParticleCollection<
        TParticle,
        TNode
    >(this IEnumerable<TParticle> self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode => new(self);
}
