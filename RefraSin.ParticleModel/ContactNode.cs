using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Records;

/// <summary>
/// Represents an immutable state of a contact node.
/// </summary>
public abstract record ContactNode : Node, IContactNode
{
    /// <inheritdoc />
    protected ContactNode(IContactNode template) : base(template)
    {
        ContactedNodeId = template.ContactedNodeId;
        ContactedParticleId = template.ContactedParticleId;
        TransferCoefficient = template.TransferCoefficient;
    }

    /// <inheritdoc />
    protected ContactNode(Guid Id,
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
        VolumeGradient
    )
    {
        ContactedParticleId = contactedParticleId;
        TransferCoefficient = transferCoefficient;
        ContactedNodeId = contactedNodeId;
    }

    /// <inheritdoc />
    public Guid ContactedParticleId { get; }

    /// <inheritdoc />
    public Guid ContactedNodeId { get; }

    /// <inheritdoc />
    public double TransferCoefficient { get; }
}