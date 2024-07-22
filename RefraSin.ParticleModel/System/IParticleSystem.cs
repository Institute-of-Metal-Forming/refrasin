using RefraSin.Graphs;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.System;

public interface IParticleSystem<out TParticle, out TNode> where TParticle : IParticle<TNode> where TNode : IParticleNode
{
    new IReadOnlyParticleCollection<TParticle, TNode> Particles { get; }

    new IReadOnlyNodeCollection<TNode> Nodes { get; }

    new IReadOnlyContactCollection<IParticleContactEdge<TParticle>> ParticleContacts { get; }

    new IReadOnlyContactCollection<IEdge<TNode>> NodeContacts { get; }
}