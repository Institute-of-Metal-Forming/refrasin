using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Record of geometry data on a node.
/// </summary>
public record NodeGeometry(Guid Id, IParticle Particle, IPolarPoint Coordinates, NodeType Type)
    : Node(Id, Particle.Id, Coordinates, Type), INodeGeometry
{
    public NodeGeometry(INodeGeometry template) : this(template.Id, template.Particle, template.Coordinates, template.Type) { }
    
    public NodeGeometry(INode template, IParticle particle) : this(template.Id, particle, template.Coordinates, template.Type) { }
}