using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores;

namespace RefraSin.ParticleModel.System;

public interface IParticleSystemWithPores<out TParticle, out TNode, out TPore>
    : IParticleSystem<TParticle, TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
    where TPore : IPore<TNode>, IPoreDensity, IPorePressure
{
    IReadOnlyVertexCollection<TPore> Pores { get; }
}
