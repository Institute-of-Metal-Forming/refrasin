using RefraSin.ParticleModel;

namespace RefraSin.Storage;

/// <summary>
/// Interface for types encapsulating data of a solution state.
/// </summary>
public interface ISolutionState
{
    /// <summary>
    /// Time coordinate.
    /// </summary>
    double Time { get; }

    /// <summary>
    /// List of all particles.
    /// </summary>
    IReadOnlyList<IParticle> ParticleStates { get; }
    
    /// <summary>
    /// List of all contacts.
    /// </summary>
    IReadOnlyList<IParticleContact> ParticleContacts { get; }
}