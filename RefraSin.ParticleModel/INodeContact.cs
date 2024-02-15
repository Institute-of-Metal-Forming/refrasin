using RefraSin.Coordinates;

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
    
    public double ContactDistance { get; }
    
    public Angle ContactDirection { get; }
    
    public NormalTangential<Angle> CenterShiftVectorDirection { get; }
    
    public NormalTangential<double> ContactDistanceGradient { get; }
    
    public NormalTangential<double> ContactDirectionGradient { get; }
}