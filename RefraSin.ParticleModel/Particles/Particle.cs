using System.Globalization;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public record Particle<TNode> : IParticle<TNode>, IParticleMeasures
    where TNode : IParticleNode
{
    public Particle(
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
        _nodes = new ReadOnlyParticleSurface<TNode>(nodes);

        MinRadius = _nodes.Min(n => n.Coordinates.R);
        MaxRadius = _nodes.Max(n => n.Coordinates.R);
        MinY = _nodes.Min(n => n.Coordinates.Absolute.Y);
        MinX = _nodes.Min(n => n.Coordinates.Absolute.X);
        MaxY = _nodes.Max(n => n.Coordinates.Absolute.Y);
        MaxX = _nodes.Max(n => n.Coordinates.Absolute.X);
    }

    public Particle(IParticle<IParticleNode> template, Func<IParticleNode, IParticle<TNode>, TNode> nodeSelector)
        : this(
            template.Id,
            template.Coordinates,
            template.RotationAngle,
            template.MaterialId,
            particle => template.Nodes.Select(n => nodeSelector(n, particle))
        ) { }

    public IReadOnlyParticleSurface<TNode> Nodes => _nodes;
    public Guid Id { get; }
    public ICartesianPoint Coordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }

    private readonly ReadOnlyParticleSurface<TNode> _nodes;

    /// <inheritdoc />
    public double MaxRadius { get; }

    /// <inheritdoc />
    public double MinRadius { get; }

    /// <inheritdoc />
    public double MinX { get; }

    /// <inheritdoc />
    public double MinY { get; }

    /// <inheritdoc />
    public double MaxX { get; }

    /// <inheritdoc />
    public double MaxY { get; }

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Particle<TNode>)} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";
}
