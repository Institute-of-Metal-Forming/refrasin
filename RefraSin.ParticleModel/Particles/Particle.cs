using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public class Particle : IParticle
{
    private readonly ReadOnlyParticleSurface<INodeGeometry> _nodes;

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
        _nodes = new ReadOnlyParticleSurface<INodeGeometry>(
            nodes.Select(node => node switch
            {
                _                          => new NodeGeometry(node, this)
            })
        );
    }

    /// <inheritdoc cref="IParticle.Nodes"/>
    public IReadOnlyParticleSurface<INodeGeometry> Nodes => _nodes;

    public Guid Id { get; }
    public ICartesianPoint Coordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }
}