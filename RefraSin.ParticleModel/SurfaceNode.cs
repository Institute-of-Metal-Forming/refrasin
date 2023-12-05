using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a surface node.
/// </summary>
public record SurfaceNode(Guid Id, Guid ParticleId, PolarPoint Coordinates, AbsolutePoint AbsoluteCoordinates, ToUpperToLower SurfaceDistance,
    ToUpperToLowerAngle SurfaceRadiusAngle, ToUpperToLowerAngle AngleDistance, ToUpperToLower Volume, NormalTangentialAngle SurfaceVectorAngle,
    NormalTangential GibbsEnergyGradient, NormalTangential VolumeGradient, ToUpperToLower SurfaceEnergy, ToUpperToLower SurfaceDiffusionCoefficient,
    double TransferCoefficient) : Node(Id, ParticleId, Coordinates), ISurfaceNode
{
    public SurfaceNode(ISurfaceNode template) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        template.AbsoluteCoordinates,
        template.SurfaceDistance,
        template.SurfaceRadiusAngle,
        template.AngleDistance,
        template.Volume,
        template.SurfaceVectorAngle,
        template.GibbsEnergyGradient,
        template.VolumeGradient,
        template.SurfaceEnergy,
        template.SurfaceDiffusionCoefficient,
        template.TransferCoefficient
    ) { }
}