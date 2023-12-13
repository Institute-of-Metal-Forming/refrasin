using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a grain boundary node.
/// </summary>
public record GrainBoundaryNode(
    Guid Id,
    Guid ParticleId,
    PolarPoint Coordinates,
    Guid ContactedParticleId,
    Guid ContactedNodeId,
    AbsolutePoint AbsoluteCoordinates,
    ToUpperToLower<double> SurfaceDistance,
    ToUpperToLower<Angle> SurfaceRadiusAngle,
    ToUpperToLower<Angle> AngleDistance,
    ToUpperToLower<double> Volume,
    NormalTangential<Angle> SurfaceVectorAngle,
    NormalTangential<double> GibbsEnergyGradient,
    NormalTangential<double> VolumeGradient,
    ToUpperToLower<double> SurfaceEnergy,
    ToUpperToLower<double> SurfaceDiffusionCoefficient,
    double TransferCoefficient,
    double ContactDistance,
    Angle ContactDirection,
    NormalTangentialRotation<Angle> CenterShiftVectorDirection,
    NormalTangentialRotation<double> ContactDistanceGradient,
    NormalTangentialRotation<double> ContactDirectionGradient
) : Node(Id, ParticleId, Coordinates, NodeType.GrainBoundaryNode), IGrainBoundaryNode
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
        template.TransferCoefficient,
        template.ContactDistance,
        template.ContactDirection,
        template.CenterShiftVectorDirection,
        template.ContactDistanceGradient,
        template.ContactDirectionGradient
    ) { }
}