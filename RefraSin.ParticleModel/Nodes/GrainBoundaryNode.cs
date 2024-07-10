using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public record GrainBoundaryNode(Guid Id, Guid ParticleId, IPolarPoint Coordinates, Guid ContactedNodeId, Guid ContactedParticleId) : IGrainBoundaryNode
{
    public GrainBoundaryNode(INodeContact template) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        template.ContactedNodeId,
        template.ContactedParticleId
    ) { }

    public GrainBoundaryNode(INode template, Guid contactedNodeId, Guid contactedParticleId) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        contactedNodeId,
        contactedParticleId
    ) { }
}