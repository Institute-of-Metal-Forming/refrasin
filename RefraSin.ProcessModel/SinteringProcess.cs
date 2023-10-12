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
    /// <param name="temperature">the process temperature</param>
    /// <param name="gasConstant">value of the universal gas constant</param>
    public SinteringProcess(
        double startTime,
        double endTime,
        IEnumerable<IParticleSpec> particleSpecs,
        IEnumerable<IMaterial> materials,
        IEnumerable<IMaterialInterface> materialInterfaces,
        double temperature,
        double gasConstant = 8.31446261815324
    )
    {
        StartTime = startTime;
        EndTime = endTime;
        Temperature = temperature;
        ParticleSpecs = particleSpecs.ToArray();
        Materials = materials.ToArray();
        MaterialInterfaces = materialInterfaces.ToArray();
        Temperature = temperature;
        GasConstant = gasConstant;
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

    /// <inheritdoc />
    public double Temperature { get; }

    /// <inheritdoc />
    public double GasConstant { get; }
}