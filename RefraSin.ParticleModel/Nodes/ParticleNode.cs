using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Record of geometry data on a node.
/// </summary>
public record ParticleNode(
    Guid Id,
    IParticle<IParticleNode> Particle,
    IPolarPoint Coordinates,
    NodeType Type
)
    : Node(Id, Particle.Id, new PolarPoint(Coordinates.Phi, Coordinates.R, Particle), Type),
        IParticleNode
{
    public ParticleNode(INode template, IParticle<IParticleNode> particle)
        : this(template.Id, particle, template.Coordinates, template.Type) { }

    private ToUpperToLower<double>? _surfaceDistance;
    private ToUpperToLower<Angle>? _surfaceRadiusAngle;
    private ToUpperToLower<Angle>? _angleDistance;
    private ToUpperToLower<double>? _volume;
    private ToUpperToLower<Angle>? _surfaceNormalAngle;
    private ToUpperToLower<Angle>? _surfaceTangentAngle;
    private ToUpperToLower<Angle>? _radiusNormalAngle;
    private ToUpperToLower<Angle>? _radiusTangentAngle;

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
}
