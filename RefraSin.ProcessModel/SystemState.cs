using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.System;

namespace RefraSin.ProcessModel;

public record SystemState(
    Guid Id,
    double Time,
    IReadOnlyParticleCollection<IParticle<IParticleNode>, IParticleNode> Particles
) : ISystemState<IParticle<IParticleNode>, IParticleNode>
{
    public SystemState(
        Guid id,
        double time,
        IParticleSystem<IParticle<IParticleNode>, IParticleNode> system
    )
        : this(id, time, system.Particles) { }

    public SystemState(Guid id, double time, IEnumerable<IParticle<IParticleNode>> particles)
        : this(id, time, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(particles)) { }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<IParticleNode> Nodes { get; } =
        Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
}
