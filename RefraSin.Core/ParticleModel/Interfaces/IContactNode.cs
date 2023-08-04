using System;

namespace RefraSin.Core.ParticleModel.Interfaces;

/// <summary>
/// Schnittstelle für Knoten, welche einen Kontakt herstellen.
/// </summary>
public interface IContactNode : INode
{
    /// <summary>
    /// Id des Partikels, welches dieser Knoten berührt.
    /// </summary>
    public Guid ContactedParticleId { get; }

    /// <summary>
    /// Id des Partikels, welches dieser Knoten berührt.
    /// </summary>
    public Guid ContactedNodeId { get; }

    /// <summary>
    /// Coefficient for volume transfer to/from the environment of the particle.
    /// </summary>
    public double TransferCoefficient { get; }
}