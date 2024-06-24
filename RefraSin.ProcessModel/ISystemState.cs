using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

/// <summary>
/// Interface for classes representing the state of a particle group at a certain position in time and process line.
/// </summary>
public interface ISystemState
{
    /// <summary>
    /// Unique ID of this state.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Time coordinate.
    /// </summary>
    double Time { get; }

    /// <summary>
    /// List of particle specifications.
    /// </summary>
    IReadOnlyParticleCollection<IParticle> Particles { get; }

    /// <summary>
    /// Collection of all nodes appearing in the particles.
    /// </summary>
    IReadOnlyNodeCollection<INode> Nodes { get; }
}