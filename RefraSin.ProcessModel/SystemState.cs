using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.System;
using RefraSin.Vertex;

namespace RefraSin.ProcessModel;

public record SystemState<TParticle, TNode>(
    Guid Id,
    double Time,
    IReadOnlyVertexCollection<TParticle> Particles
) : ISystemState<TParticle, TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    public SystemState(Guid id, double time, IParticleSystem<TParticle, TNode> system)
        : this(id, time, system.Particles) { }

    public SystemState(Guid id, double time, IEnumerable<TParticle> particles)
        : this(id, time, new ParticleSystem<TParticle, TNode>(particles)) { }

    /// <inheritdoc />
    public IReadOnlyVertexCollection<TNode> Nodes { get; } =
        Particles.SelectMany(p => p.Nodes).ToReadOnlyVertexCollection();
}

public record SystemState(
    Guid Id,
    double Time,
    IReadOnlyVertexCollection<IParticle<IParticleNode>> Particles
) : SystemState<IParticle<IParticleNode>, IParticleNode>(Id, Time, Particles)
{
    public SystemState(
        Guid id,
        double time,
        IParticleSystem<IParticle<IParticleNode>, IParticleNode> system
    )
        : this(id, time, system.Particles) { }

    public SystemState(Guid id, double time, IEnumerable<IParticle<IParticleNode>> particles)
        : this(id, time, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(particles)) { }
}
