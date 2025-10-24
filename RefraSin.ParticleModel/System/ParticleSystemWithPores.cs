using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.System;

public record ParticleSystemWithPores<TParticle, TNode, TPore>(
    IReadOnlyVertexCollection<TParticle> Particles,
    IReadOnlyVertexCollection<TPore> Pores
) : ParticleSystem<TParticle, TNode>(Particles), IParticleSystemWithPores<TParticle, TNode, TPore>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
    where TPore : IPore<TNode>
{
    public ParticleSystemWithPores(IParticleSystemWithPores<TParticle, TNode, TPore> system)
        : this(system.Particles, system.Pores) { }

    public ParticleSystemWithPores(IEnumerable<TParticle> particles, IEnumerable<TPore> pores)
        : this(particles.ToReadOnlyVertexCollection(), pores.ToReadOnlyVertexCollection()) { }
}
