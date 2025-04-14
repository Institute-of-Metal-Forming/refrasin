using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.System;

public class ParticleSystem<TParticle, TNode> : IParticleSystem<TParticle, TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    public ParticleSystem(IEnumerable<TParticle> particles)
    {
        Particles = particles.ToReadOnlyParticleCollection<TParticle, TNode>();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
    }

    /// <inheritdoc />
    public IReadOnlyParticleCollection<TParticle, TNode> Particles { get; }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<TNode> Nodes { get; }
}
