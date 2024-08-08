using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction.ParticleModel;

internal class Node : IParticleNode
{
    private ToUpperToLower<double>? _surfaceDistance;
    private ToUpperToLower<Angle>? _surfaceRadiusAngle;
    private ToUpperToLower<Angle>? _angleDistance;
    private ToUpperToLower<double>? _volume;
    private ToUpperToLower<Angle>? _surfaceNormalAngle;
    private ToUpperToLower<Angle>? _surfaceTangentAngle;
    private ToUpperToLower<Angle>? _radiusNormalAngle;
    private ToUpperToLower<Angle>? _radiusTangentAngle;

    public Node(Guid id, AbsolutePoint coordinates, NodeType type, IParticle<Node> particle)
    {
        Id = id;
        Coordinates = new PolarPoint(coordinates, particle);
        Type = type;
        Particle = particle;
    }

    public Node(INode node, IParticle<Node> particle)
    {
        Id = node.Id;
        Coordinates = new PolarPoint(node.Coordinates.Phi, node.Coordinates.R, particle);
        Type = node.Type;
        Particle = particle;
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public IParticle<Node> Particle { get; }

    IParticle<IParticleNode> IParticleNode.Particle => Particle;

    /// <inheritdoc />
    public Guid ParticleId => Particle.Id;

    /// <inheritdoc />
    public IPolarPoint Coordinates { get; }

    /// <inheritdoc />
    public NodeType Type { get; }

    /// <inheritdoc />
    public ToUpperToLower<double> SurfaceDistance => _surfaceDistance ??= this.SurfaceDistance();

    /// <inheritdoc />
    public ToUpperToLower<Angle> SurfaceRadiusAngle =>
        _surfaceRadiusAngle ??= this.SurfaceRadiusAngle();

    /// <inheritdoc />
    public ToUpperToLower<Angle> AngleDistance => _angleDistance ??= this.AngleDistance();

    /// <inheritdoc />
    public ToUpperToLower<double> Volume => _volume ??= this.Volume();

    /// <inheritdoc />
    public ToUpperToLower<Angle> SurfaceNormalAngle =>
        _surfaceNormalAngle ??= this.SurfaceNormalAngle();

    /// <inheritdoc />
    public ToUpperToLower<Angle> SurfaceTangentAngle =>
        _surfaceTangentAngle ??= this.SurfaceTangentAngle();

    /// <inheritdoc />
    public ToUpperToLower<Angle> RadiusNormalAngle =>
        _radiusNormalAngle ??= this.RadiusNormalAngle();

    /// <inheritdoc />
    public ToUpperToLower<Angle> RadiusTangentAngle =>
        _radiusTangentAngle ??= this.RadiusTangentAngle();

    public Node Upper => Particle.Nodes.UpperNeighborOf(this);

    public Node Lower => Particle.Nodes.LowerNeighborOf(this);
}
