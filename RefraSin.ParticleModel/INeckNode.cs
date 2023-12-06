namespace RefraSin.ParticleModel;

/// <summary>
/// Interface für Halsknoten.
/// </summary>
public interface INeckNode : INode, INodeContact, INodeGeometry, INodeGradients, INodeMaterialProperties
{
    public Guid OppositeNeckNodeId { get; }
}