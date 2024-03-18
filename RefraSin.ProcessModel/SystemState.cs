using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

public class SystemState(
    double time,
    IEnumerable<IParticle> particles,
    IEnumerable<IMaterial> materials,
    IEnumerable<IMaterialInterface> materialInterfaces)
    : ISystemState
{
    /// <inheritdoc />
    public double Time { get; } = time;

    /// <inheritdoc />
    public IReadOnlyList<IParticle> Particles { get; } = particles.ToArray();

    /// <inheritdoc />
    public IReadOnlyList<IMaterial> Materials { get; } = materials.ToArray();

    /// <inheritdoc />
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; } = materialInterfaces.ToArray();
}