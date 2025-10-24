using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.System;
using RefraSin.Vertex;

namespace RefraSin.ProcessModel;

public record SystemStateWithPores<TParticle, TNode, TPore>(
    Guid Id,
    double Time,
    IReadOnlyVertexCollection<TParticle> Particles,
    IReadOnlyVertexCollection<TPore> Pores
)
    : SystemState<TParticle, TNode>(Id, Time, Particles),
        ISystemStateWithPores<TParticle, TNode, TPore>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
    where TPore : IPoreState<TNode>
{
    public SystemStateWithPores(
        Guid id,
        double time,
        IParticleSystem<TParticle, TNode> system,
        IEnumerable<TPore> pores
    )
        : this(id, time, system.Particles, pores.ToReadOnlyVertexCollection()) { }

    public SystemStateWithPores(
        Guid id,
        double time,
        IEnumerable<TParticle> particles,
        IEnumerable<TPore> pores
    )
        : this(
            id,
            time,
            new ParticleSystem<TParticle, TNode>(particles),
            pores.ToReadOnlyVertexCollection()
        ) { }
}

public record SystemStateWithPores(
    Guid Id,
    double Time,
    IReadOnlyVertexCollection<IParticle<IParticleNode>> Particles,
    IReadOnlyVertexCollection<IPoreState<IParticleNode>> Pores
)
    : SystemStateWithPores<IParticle<IParticleNode>, IParticleNode, IPoreState<IParticleNode>>(
        Id,
        Time,
        Particles,
        Pores
    )
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
