using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.System;

namespace RefraSin.ProcessModel;

public record SystemStateWithPores(
    Guid Id,
    double Time,
    IReadOnlyVertexCollection<IParticle<IParticleNode>> Particles,
    IReadOnlyVertexCollection<IPoreState<IParticleNode>> Pores
)
    : SystemState(Id, Time, Particles),
        IParticleSystemWithPores<IParticle<IParticleNode>, IParticleNode, IPoreState<IParticleNode>>
{
    public SystemStateWithPores(
        Guid id,
        double time,
        IParticleSystem<IParticle<IParticleNode>, IParticleNode> system,
        IEnumerable<IPoreState<IParticleNode>> pores
    )
        : this(id, time, system.Particles, pores.ToReadOnlyVertexCollection()) { }

    public SystemStateWithPores(
        Guid id,
        double time,
        IEnumerable<IParticle<IParticleNode>> particles,
        IEnumerable<IPoreState<IParticleNode>> pores
    )
        : this(
            id,
            time,
            new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(particles),
            pores.ToReadOnlyVertexCollection()
        ) { }
}
