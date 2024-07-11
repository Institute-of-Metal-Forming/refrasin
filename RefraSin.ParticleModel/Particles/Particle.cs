using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public record Particle : IParticle
{
    private readonly ReadOnlyParticleSurface<IParticleNode> _nodes;

    public Particle(
        Guid id,
        ICartesianPoint centerCoordinates,
        Angle rotationAngle,
        Guid materialId,
        Func<IParticle, IEnumerable<IParticleNode>> nodesFactory
    )
    {
        Id = id;
        Coordinates = centerCoordinates;
        RotationAngle = rotationAngle;
        MaterialId = materialId;
        var nodes = nodesFactory(this).ToArray();
        if (nodes.Any(n => !ReferenceEquals(n.Particle, this)))
            throw new InvalidOperationException(
                "All nodes produced by the factory must be associated with the created particle instance."
            );
        _nodes = new ReadOnlyParticleSurface<IParticleNode>(nodes);
    }

    public Particle(IParticle template)
        : this(
            template.Id,
            template.Coordinates,
            template.RotationAngle,
            template.MaterialId,
            particle => template.Nodes.Select(n => new ParticleNode(n, particle))
        ) { }

    public IReadOnlyParticleSurface<IParticleNode> Nodes => _nodes;
    public Guid Id { get; }
    public ICartesianPoint Coordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }
}
