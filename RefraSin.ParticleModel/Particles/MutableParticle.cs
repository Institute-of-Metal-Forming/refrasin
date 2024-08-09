using System.Globalization;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public class MutableParticle<TNode> : IMutableParticle<TNode>
    where TNode : IParticleNode
{
    private readonly IParticleSurface<TNode> _surface;

    public MutableParticle(
        Guid id,
        ICartesianPoint centerCoordinates,
        Angle rotationAngle,
        Guid materialId,
        Func<IParticle<TNode>, IEnumerable<TNode>> nodesFactory
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
        _surface = new ParticleSurface<TNode>(nodes);
    }

    public MutableParticle(IParticle<IParticleNode> template, Func<IParticleNode, IParticle<TNode>, TNode> nodeSelector)
        : this(
            template.Id,
            template.Coordinates,
            template.RotationAngle,
            template.MaterialId,
            particle => template.Nodes.Select(n => nodeSelector(n, particle))
        ) { }

    public IReadOnlyParticleSurface<TNode> Nodes => _surface;
    public Guid Id { get; }
    public ICartesianPoint Coordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Particle<TNode>)} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";

    /// <inheritdoc />
    public IParticleSurface<TNode> Surface => _surface;
}
