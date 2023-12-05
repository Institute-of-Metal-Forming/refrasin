using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a grain boundary node.
/// </summary>
public record GrainBoundaryNode(Guid Id, Guid ParticleId, PolarPoint Coordinates, Guid ContactedParticleId, Guid ContactedNodeId,
    AbsolutePoint AbsoluteCoordinates, ToUpperToLower SurfaceDistance, ToUpperToLowerAngle SurfaceRadiusAngle, ToUpperToLowerAngle AngleDistance,
    ToUpperToLower Volume, NormalTangentialAngle SurfaceVectorAngle, NormalTangential GibbsEnergyGradient, NormalTangential VolumeGradient,
    ToUpperToLower SurfaceEnergy, ToUpperToLower SurfaceDiffusionCoefficient, double TransferCoefficient) : Node(Id, ParticleId, Coordinates),
    IGrainBoundaryNode
{
    public GrainBoundaryNode(IGrainBoundaryNode template) : this(
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