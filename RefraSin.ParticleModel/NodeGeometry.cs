using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

public record NodeGeometry(Guid Id, IParticle Particle, PolarPoint Coordinates, NodeType Type)
    : Node(Id, Particle.Id, Coordinates, Type), INodeGeometry
{
    public NodeGeometry(INodeGeometry template) : this(template.Id, template.Particle, template.Coordinates, template.Type) { }
    
    public NodeGeometry(INode template, IParticle particle) : this(template.Id, particle, template.Coordinates, template.Type) { }
}