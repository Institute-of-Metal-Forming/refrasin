using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a neck node.
/// </summary>
public record NeckNode(
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
    ToUpperToLower<Angle> SurfaceNormalAngle,
    ToUpperToLower<Angle> SurfaceTangentAngle,
    NormalTangential<double> GibbsEnergyGradient,
    NormalTangential<double> VolumeGradient,
    ToUpperToLower<double> SurfaceEnergy,
    ToUpperToLower<double> SurfaceDiffusionCoefficient,
    double ContactDistance,
    Angle ContactDirection,
    Angle AngleDistanceFromContactDirection,
    NormalTangential<Angle> CenterShiftVectorDirection,
    NormalTangential<double> ContactDistanceGradient,
    NormalTangential<double> ContactDirectionGradient
) : Node(Id, ParticleId, Coordinates, NodeType.NeckNode), INeckNode
{
    public NeckNode(INeckNode template)
        : this(
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
            template.SurfaceNormalAngle,
            template.SurfaceTangentAngle,
            template.GibbsEnergyGradient,
            template.VolumeGradient,
            template.SurfaceEnergy,
            template.SurfaceDiffusionCoefficient,
            template.ContactDistance,
            template.ContactDirection,
            template.AngleDistanceFromContactDirection,
            template.CenterShiftVectorDirection,
            template.ContactDistanceGradient,
            template.ContactDirectionGradient
        ) { }
}
