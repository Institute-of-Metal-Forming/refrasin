using RefraSin.Graphs;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.System;

public class ParticleSystem<TParticle, TNode> : IParticleSystem<TParticle, TNode> where TParticle : IParticle<TNode> where TNode : IParticleNode
{
    public ParticleSystem(IEnumerable<TParticle> particles)
    {
        Particles = particles.ToReadOnlyParticleCollection<TParticle, TNode>();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
        var contactNodes = Nodes.Where(n => n.Type is Neck or GrainBoundary).ToArray();
        NodeContacts = contactNodes.Select(node => new Edge<TNode>(node, node.FindContactedNodeByCoordinates(contactNodes), false)).ToReadOnlyContactCollection();
        ParticleContacts = NodeContacts
            .Select(nc => new ParticleContactEdge<TParticle>(Particles[nc.From.ParticleId], Particles[nc.To.ParticleId]))
            .Distinct()
            .ToReadOnlyContactCollection();
    }

    public ParticleSystem(IEnumerable<TParticle> particles, IEnumerable<IEdge> particleContacts, IEnumerable<IEdge> nodeContacts)
    {
        Particles = particles.ToReadOnlyParticleCollection<TParticle, TNode>();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
        NodeContacts = nodeContacts.Select(nc => new Edge<TNode>(Nodes[nc.From], Nodes[nc.To], true)).ToReadOnlyContactCollection();
        ParticleContacts = particleContacts.Select(pc => new ParticleContactEdge<TParticle>(Particles[pc.From], Particles[pc.To])).ToReadOnlyContactCollection();
    }

    /// <inheritdoc />
    public IReadOnlyParticleCollection<TParticle, TNode> Particles { get; }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<TNode> Nodes { get; }

    /// <inheritdoc />
    public IReadOnlyContactCollection<IParticleContactEdge<TParticle>> ParticleContacts { get; }

    /// <inheritdoc />
    public IReadOnlyContactCollection<IEdge<TNode>> NodeContacts { get; }
}