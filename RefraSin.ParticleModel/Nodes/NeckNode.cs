using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public record NeckNode(Guid Id, Guid ParticleId, IPolarPoint Coordinates, Guid ContactedNodeId, Guid ContactedParticleId) : INeckNode
{
    public NeckNode(INodeContact template) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        template.ContactedNodeId,
        template.ContactedParticleId
    ) { }

    public NeckNode(INode template, Guid contactedNodeId, Guid contactedParticleId) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        contactedNodeId,
        contactedParticleId
    ) { }
}