using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;

namespace RefraSin.Storage.ParticleModel;

/// <summary>
/// Represents an immutable record of a grain boundary node.
/// </summary>
public record GrainBoundaryNodeRecord : ContactNodeRecord, IGrainBoundaryNode
{
    /// <inheritdoc />
    public GrainBoundaryNodeRecord(IGrainBoundaryNode template) : base(template) { }

    /// <inheritdoc />
    public GrainBoundaryNodeRecord(Guid Id,
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
        double transferCoefficient
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
    ) { }
}