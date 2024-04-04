using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

public class SystemState : ISystemState
{
    public SystemState(
        Guid id,
        double time,
        IEnumerable<IParticle> particles,
        IEnumerable<IMaterial> materials,
        IEnumerable<IMaterialInterface> materialInterfaces
    )
        : this(id, time, particles, materials, materialInterfaces, Array.Empty<IParticleContact>())
    { }

    public SystemState(
        Guid id,
        double time,
        IEnumerable<IParticle> particles,
        IEnumerable<IMaterial> materials,
        IEnumerable<IMaterialInterface> materialInterfaces,
        IEnumerable<IParticleContact> particleContacts
    )
    {
        Id = id;
        Time = time;
        Particles = particles.Select(s => s as Particle ?? new Particle(s)).ToParticleCollection();
        Contacts = particleContacts
            .Select(c => c as ParticleContact ?? new ParticleContact(c))
            .ToParticleContactCollection();
        Nodes = new ReadOnlyNodeCollection<INode>(Particles.SelectMany(p => p.Nodes));
        Materials = materials.ToArray();
        MaterialInterfaces = materialInterfaces.ToArray();
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc />
    public IReadOnlyParticleCollection<IParticle> Particles { get; }

    /// <inheritdoc />
    public IReadOnlyNodeCollection<INode> Nodes { get; }

    /// <inheritdoc />
    public IReadOnlyParticleContactCollection<IParticleContact> Contacts { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterial> Materials { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }
}
