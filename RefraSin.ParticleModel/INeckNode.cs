namespace RefraSin.ParticleModel;

/// <summary>
/// Interface für Halsknoten.
/// </summary>
public interface INeckNode : IContactNode
{
    public Guid OppositeNeckNodeId { get; }
}