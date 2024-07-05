using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a surface node.
/// </summary>
public record SurfaceNode(
    Guid Id,
    IParticle Particle,
    PolarPoint Coordinates,
    AbsolutePoint AbsoluteCoordinates,
    ToUpperToLower<double> SurfaceDistance,
    ToUpperToLower<Angle> SurfaceRadiusAngle,
    ToUpperToLower<Angle> AngleDistance,
    ToUpperToLower<double> Volume,
    ToUpperToLower<Angle> SurfaceNormalAngle,
    ToUpperToLower<Angle> SurfaceTangentAngle,
    NormalTangential<double> GibbsEnergyGradient,
    NormalTangential<double> VolumeGradient,
    ToUpperToLower<double> SurfaceEnergy,
    ToUpperToLower<double> SurfaceDiffusionCoefficient
   ) : Node(Id, Particle.Id, Coordinates, NodeType.Surface), ISurfaceNode
{
    public SurfaceNode(ISurfaceNode template) : this(
        template.Id,
        template.Particle,
        template.Coordinates,
        template.AbsoluteCoordinates,
        template.SurfaceDistance,
        template.SurfaceRadiusAngle,
        template.AngleDistance,
        template.Volume,
        template.SurfaceNormalAngle,
        template.SurfaceTangentAngle,
        template.GibbsEnergyGradient,
        template.VolumeGradient,
        template.SurfaceEnergy,
        template.SurfaceDiffusionCoefficient
    ) { }
}