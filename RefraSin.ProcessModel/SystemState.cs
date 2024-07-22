using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;

namespace RefraSin.ProcessModel;

public class SystemState : ISystemState
{
    public SystemState(
        Guid id,
        double time,
        IParticleSystem<IParticle<IParticleNode>, IParticleNode> system)
    {
        Id = id;
        Time = time;
        Particles = system.Particles;
        Nodes = system.Nodes;
        ParticleContacts = system.ParticleContacts;
        NodeContacts = system.NodeContacts;
    }

    public SystemState(
        Guid id,
        double time,
        IEnumerable<IParticle<IParticleNode>> particles) : this(id, time, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(particles)) { }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc />
    public IReadOnlyParticleCollection<IParticle<IParticleNode>, IParticleNode> Particles { get; }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<IParticleNode> Nodes { get; }

    /// <inheritdoc />
    public IReadOnlyContactCollection<IParticleContactEdge<IParticle<IParticleNode>>> ParticleContacts { get; }

    /// <inheritdoc />
    public IReadOnlyContactCollection<IEdge<IParticleNode>> NodeContacts { get; }
}