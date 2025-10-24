using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.System;

public record ParticleSystem<TParticle, TNode>(IReadOnlyVertexCollection<TParticle> Particles)
    : IParticleSystem<TParticle, TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    public ParticleSystem(IParticleSystem<TParticle, TNode> system)
        : this(system.Particles) { }

    public ParticleSystem(IEnumerable<TParticle> particles)
        : this(particles.ToReadOnlyVertexCollection()) { }

    /// <inheritdoc />
    public IReadOnlyVertexCollection<TNode> Nodes { get; } =
        Particles.SelectMany(p => p.Nodes).ToReadOnlyVertexCollection();
}
