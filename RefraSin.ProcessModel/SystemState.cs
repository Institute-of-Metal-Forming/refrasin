using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

public class SystemState : ISystemState
{
    public SystemState(
        Guid id,
        double time,
        IEnumerable<IParticle> particles
    )
    {
        Id = id;
        Time = time;
        Particles = particles.Select(s => s as Particle ?? new Particle(s)).ToParticleCollection();
        Nodes = new ReadOnlyNodeCollection<INode>(Particles.SelectMany(p => p.Nodes));
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc />
    public IReadOnlyParticleCollection<IParticle> Particles { get; }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<INode> Nodes { get; }
}