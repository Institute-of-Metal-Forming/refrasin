using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

/// <summary>
/// Data structure representing a sintering process.
/// </summary>
public class SinteringProcess : ISinteringProcess
{
    /// <summary>
    /// Creates a new sintering process.
    /// </summary>
    /// <param name="startTime">time coordinate of the process start</param>
    /// <param name="endTime">time coordinate of the process end</param>
    /// <param name="particleSpecs">enumerable of particle specifications</param>
    /// <param name="materials">enumerable of materials appearing</param>
    /// <param name="materialInterfaces">enumerable of material interfaces appearing</param>
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