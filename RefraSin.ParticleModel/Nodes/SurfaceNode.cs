using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public record SurfaceNode(Guid Id, Guid ParticleId, IPolarPoint Coordinates) : ISurfaceNode
{
    public SurfaceNode(INode template) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates
    ) { }
}