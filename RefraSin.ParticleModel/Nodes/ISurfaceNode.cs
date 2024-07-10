namespace RefraSin.ParticleModel.Nodes;

public interface ISurfaceNode : INode
{
    NodeType INode.Type => NodeType.Surface;
}