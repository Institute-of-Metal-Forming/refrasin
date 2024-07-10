namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Interface für Halsknoten.
/// </summary>
public interface INeckNode : INodeContact
{
    NodeType INode.Type => NodeType.Neck;
}