using MathNet.Numerics.Distributions;

namespace RefraSin.ParticleModel;

/// <summary>
/// Schnittstelle für Knoten, welche einen Kontakt herstellen.
/// </summary>
public interface INodeContact
{
    /// <summary>
    /// Id des Partikels, welches dieser Knoten berührt.
    /// </summary>
    public Guid ContactedParticleId { get; }

    /// <summary>
    /// Id des Partikels, welches dieser Knoten berührt.
    /// </summary>
    public Guid ContactedNodeId { get; }
}