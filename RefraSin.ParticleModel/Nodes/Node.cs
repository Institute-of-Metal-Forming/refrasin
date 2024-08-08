using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public record Node(Guid Id, Guid ParticleId, IPolarPoint Coordinates, NodeType Type) : INode
{
    public Node(INode template)
        : this(template.Id, template.ParticleId, template.Coordinates, template.Type) { }
}
