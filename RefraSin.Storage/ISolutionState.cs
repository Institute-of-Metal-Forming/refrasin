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
    IReadOnlyParticleCollection<IParticle> Particles { get; }

    /// <summary>
    /// List of all nodes.
    /// </summary>
    IReadOnlyNodeCollection<INode> Nodes { get; }

    /// <summary>
    /// List of all contacts.
    /// </summary>
    IReadOnlyParticleContactCollection<IParticleContact> Contacts { get; }
}