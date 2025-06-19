using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.System;

public class ParticleSystem<TParticle, TNode> : IParticleSystem<TParticle, TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    public ParticleSystem(IEnumerable<TParticle> particles)
    {
        Particles = particles.ToReadOnlyVertexCollection();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyVertexCollection();
    }

    /// <inheritdoc />
    public IReadOnlyVertexCollection<TParticle> Particles { get; }

    /// <inheritdoc />
    public IReadOnlyVertexCollection<TNode> Nodes { get; }
}
