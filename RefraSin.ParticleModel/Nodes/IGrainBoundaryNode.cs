namespace RefraSin.ParticleModel.Nodes
{
    public interface IGrainBoundaryNode : INodeContact
    {
        NodeType INode.Type => NodeType.GrainBoundary;
    }
}