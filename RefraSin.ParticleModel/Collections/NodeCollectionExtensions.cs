using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Collections;

public static class NodeCollectionExtensions
{
    public static ReadOnlyParticleSurface<TNode> ToParticleSurface<TNode>(this IEnumerable<TNode> source) where TNode : INode => new(source);

    public static Dictionary<Guid, TNode> ToDictionaryById<TNode>(this IEnumerable<TNode> source) where TNode : INode =>
        source.ToDictionary(n => n.Id, n => n);
    
    /// <summary>
    /// Creates a new <see cref="ReadOnlyNodeCollection{TNode}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyNodeCollection<TNode> ToNodeCollection<TNode>(this IEnumerable<TNode> self) where TNode : INode => new(self);

    /// <summary>
    /// Creates a new <see cref="ReadOnlyParticleCollection{TParticle}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyParticleCollection<TParticle> ToParticleCollection<TParticle>(this IEnumerable<TParticle> self)
        where TParticle : IParticle => new(self);

    /// <summary>
    /// Creates a new <see cref="ReadOnlyParticleContactCollection{TParticleContact}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyParticleContactCollection<TParticleContact>
        ToParticleContactCollection<TParticleContact>(this IEnumerable<TParticleContact> self) where TParticleContact : IParticleContact => new(self);
}