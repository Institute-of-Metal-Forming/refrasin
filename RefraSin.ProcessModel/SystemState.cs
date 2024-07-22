using RefraSin.Graphs;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.System;

namespace RefraSin.ProcessModel;

public record SystemState(
    Guid Id,
    double Time,
    IReadOnlyParticleCollection<IParticle<IParticleNode>, IParticleNode> Particles,
    IReadOnlyContactCollection<IParticleContactEdge<IParticle<IParticleNode>>> ParticleContacts,
    IReadOnlyContactCollection<IEdge<IParticleNode>> NodeContacts
) : ISystemState
{
    public SystemState(
        Guid id,
        double time,
        IParticleSystem<IParticle<IParticleNode>, IParticleNode> system) : this(id, time, system.Particles, system.ParticleContacts,
        system.NodeContacts) { }

    public SystemState(Guid id, double time, IEnumerable<IParticle<IParticleNode>> particles,
        IEnumerable<IParticleContactEdge<IParticle<IParticleNode>>> particleContacts, IEnumerable<IEdge<IParticleNode>> nodeContacts) : this(id, time,
        particles.ToReadOnlyParticleCollection<IParticle<IParticleNode>, IParticleNode>(), particleContacts.ToReadOnlyContactCollection(),
        nodeContacts.ToReadOnlyContactCollection()) { }

    public SystemState(
        Guid id,
        double time,
        IEnumerable<IParticle<IParticleNode>> particles) : this(id, time, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(particles)) { }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<IParticleNode> Nodes { get; } = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
}