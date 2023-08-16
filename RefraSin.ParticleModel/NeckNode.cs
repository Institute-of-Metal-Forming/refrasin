using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Records;

/// <summary>
/// Represents an immutable record of a neck node.
/// </summary>
public record NeckNode : ContactNode, INeckNode
{
    /// <inheritdoc />
    public NeckNode(INeckNode template) : base(template)
    {
        OppositeNeckNodeId = template.OppositeNeckNodeId;
    }

    /// <inheritdoc />
    public NeckNode(Guid Id,
        Guid ParticleId,
        PolarPoint Coordinates,
        AbsolutePoint AbsoluteCoordinates,
        ToUpperToLower SurfaceDistance,
        ToUpperToLowerAngle SurfaceRadiusAngle,
        ToUpperToLowerAngle AngleDistance,
        ToUpperToLower Volume,
        NormalTangentialAngle SurfaceAngle,
        ToUpperToLower SurfaceEnergy,
        ToUpperToLower SurfaceDiffusionCoefficient,
        NormalTangential GibbsEnergyGradient,
        NormalTangential VolumeGradient,
        Guid contactedParticleId,
        Guid contactedNodeId,
        double transferCoefficient,
        Guid oppositeNeckNodeId
    ) : base(
        Id,
        ParticleId,
        Coordinates,
        AbsoluteCoordinates,
        SurfaceDistance,
        SurfaceRadiusAngle,
        AngleDistance,
        Volume,
        SurfaceAngle,
        SurfaceEnergy,
        SurfaceDiffusionCoefficient,
        GibbsEnergyGradient,
        VolumeGradient,
        contactedParticleId,
        contactedNodeId,
        transferCoefficient
    )
    {
        OppositeNeckNodeId = oppositeNeckNodeId;
    }

    /// <inheritdoc />
    public Guid OppositeNeckNodeId { get; }
}