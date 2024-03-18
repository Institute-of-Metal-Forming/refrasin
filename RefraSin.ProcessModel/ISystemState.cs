using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

/// <summary>
/// Interface for classes representing the state of a particle group at a certain position in time and process line.
/// </summary>
public interface ISystemState
{
    /// <summary>
    /// Time coordinate.
    /// </summary>
    double Time { get; }
    
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
}