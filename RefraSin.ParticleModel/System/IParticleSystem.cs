using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.System;

public interface IParticleSystem<out TParticle, out TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    IReadOnlyVertexCollection<TParticle> Particles { get; }

    IReadOnlyVertexCollection<TNode> Nodes { get; }
}
