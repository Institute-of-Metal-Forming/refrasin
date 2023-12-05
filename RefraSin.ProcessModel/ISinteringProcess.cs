using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

/// <summary>
/// Interface that defines a data structure representing a sintering process.
/// </summary>
public interface ISinteringProcess
{
    /// <summary>
    /// Time coordinate of the process start.
    /// </summary>
    public double StartTime { get; }

    /// <summary>
    /// Time coordinate of the process end.
    /// </summary>
    public double EndTime { get; }

    /// <summary>
    /// List of particle specifications.
    /// </summary>
    public IReadOnlyList<IParticle> Particles { get; }

    /// <summary>
    /// List of materials appearing in the process.
    /// </summary>
    public IReadOnlyList<IMaterial> Materials { get; }

    /// <summary>
    /// List of material interfaces appearing in the process.
    /// </summary>
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }

    /// <summary>
    /// Constant process temperature.
    /// </summary>
    public double Temperature { get; }

    /// <summary>
    /// Universal gas constant R.
    /// </summary>
    public double GasConstant { get; }
}