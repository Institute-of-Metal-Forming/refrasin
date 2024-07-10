using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public class Particle : IParticle
{
    private readonly ReadOnlyParticleSurface<IParticleNode> _nodes;

    public Particle(IParticle template) : this(
        template.Id,
        template.Coordinates,
        template.RotationAngle,
        template.MaterialId,
        template.Nodes
    ) { }

    public Particle(Guid id, ICartesianPoint centerCoordinates, Angle rotationAngle, Guid materialId, IReadOnlyList<INode> nodes)
    {
        Id = id;
        Coordinates = centerCoordinates;
        RotationAngle = rotationAngle;
        MaterialId = materialId;
        _nodes = new ReadOnlyParticleSurface<IParticleNode>(
            nodes.Select(node => node switch
            {
                _                          => new ParticleNode(node, this)
            })
        );
    }
    public Particle(Guid id, ICartesianPoint centerCoordinates, Angle rotationAngle, Guid materialId, Func<IParticle,IEnumerable<IParticleNode>> nodeFactory)
    {
        Id = id;
        Coordinates = centerCoordinates;
        RotationAngle = rotationAngle;
        MaterialId = materialId;
        _nodes = new ReadOnlyParticleSurface<IParticleNode>( nodeFactory(this) );
    }

    /// <inheritdoc cref="IParticle.Nodes"/>
    public IReadOnlyParticleSurface<IParticleNode> Nodes => _nodes;

    public Guid Id { get; }
    public ICartesianPoint Coordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }
}