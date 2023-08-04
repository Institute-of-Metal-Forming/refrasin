using System;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.ParticleModel.Records;

/// <summary>
/// Represents an immutable record of a neck node.
/// </summary>
public record NeckNodeRecord : ContactNodeRecord, INeckNode
{
    /// <inheritdoc />
    public NeckNodeRecord(INeckNode template) : base(template)
    {
        OppositeNeckNodeId = template.OppositeNeckNodeId;
    }

    /// <inheritdoc />
    public NeckNodeRecord(Guid Id,
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
        transferCoefficient
    )
    {
        OppositeNeckNodeId = oppositeNeckNodeId;
    }

    /// <inheritdoc />
    public Guid OppositeNeckNodeId { get; }
}