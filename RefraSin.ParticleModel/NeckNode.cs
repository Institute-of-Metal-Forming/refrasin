using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a neck node.
/// </summary>
public record NeckNode(Guid Id, Guid ParticleId, PolarPoint Coordinates, Guid ContactedParticleId, Guid ContactedNodeId,
    AbsolutePoint AbsoluteCoordinates, ToUpperToLower SurfaceDistance, ToUpperToLowerAngle SurfaceRadiusAngle, ToUpperToLowerAngle AngleDistance,
    ToUpperToLower Volume, NormalTangentialAngle SurfaceVectorAngle, NormalTangential GibbsEnergyGradient, NormalTangential VolumeGradient,
    ToUpperToLower SurfaceEnergy, ToUpperToLower SurfaceDiffusionCoefficient, double TransferCoefficient) : Node(Id, ParticleId, Coordinates),
    INeckNode
{
    public NeckNode(INeckNode template) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        template.ContactedParticleId,
        template.ContactedNodeId,
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