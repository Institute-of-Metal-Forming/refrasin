using RefraSin.Graphs;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Interface for nodes being in contact with another.
/// </summary>
public interface INodeContact
{
    /// <summary>
    /// ID of the other node, the contact is built to.
    /// </summary>
    Guid ContactedNodeId { get; }

    /// <summary>
    /// ID of the other particle, the contact is built to.
    /// </summary>
    Guid ContactedParticleId { get; }
}