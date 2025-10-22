using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.System;

public class ParticleSystemWithPores<TParticle, TNode, TPore>(
    IEnumerable<TParticle> particles,
    IEnumerable<TPore> pores
) : ParticleSystem<TParticle, TNode>(particles), IParticleSystemWithPores<TParticle, TNode, TPore>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
    where TPore : IPore<TNode>, IPoreDensity, IPorePressure
{
    public IReadOnlyVertexCollection<TPore> Pores { get; } = pores.ToReadOnlyVertexCollection();
}
