using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

public class SinteringProcess : ISinteringProcess
{
    public SinteringProcess(
        double startTime,
        double endTime,
        IEnumerable<IParticleSpec> particleSpecs,
        IEnumerable<IMaterial> materials,
        IEnumerable<IMaterialInterface> materialInterfaces
    )
    {
        StartTime = startTime;
        EndTime = endTime;
        ParticleSpecs = particleSpecs.ToArray();
        Materials = materials.ToArray();
        MaterialInterfaces = materialInterfaces.ToArray();
    }

    /// <inheritdoc />
    public double StartTime { get; }

    /// <inheritdoc />
    public double EndTime { get; }

    /// <inheritdoc />
    public IReadOnlyList<IParticleSpec> ParticleSpecs { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterial> Materials { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }
}