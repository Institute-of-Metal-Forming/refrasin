using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;

namespace RefraSin.Storage.ParticleModel;

/// <summary>
/// Represents an immutable record of a node.
/// </summary>
public abstract record NodeRecord(
    Guid Id,
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
    NormalTangential VolumeGradient
) : INode
{
    /// <summary>
    /// Creates a new instance by shallow copying from a template.
    /// </summary>
    /// <param name="template">the template to copy from</param>
    protected NodeRecord(INode template) : this(
        template.Id,
        template.ParticleId,
        template.Coordinates,
        template.AbsoluteCoordinates,
        template.SurfaceDistance,
        template.SurfaceRadiusAngle,
        template.AngleDistance,
        template.Volume,
        template.SurfaceAngle,
        template.SurfaceEnergy,
        template.SurfaceDiffusionCoefficient,
        template.GibbsEnergyGradient,
        template.VolumeGradient
    ) { }

    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public Guid ParticleId { get; } = ParticleId;

    /// <inheritdoc />
    public PolarPoint Coordinates { get; } = Coordinates;

    /// <inheritdoc />
    public AbsolutePoint AbsoluteCoordinates { get; } = AbsoluteCoordinates;

    /// <inheritdoc />
    public ToUpperToLower SurfaceDistance { get; } = SurfaceDistance;

    /// <inheritdoc />
    public ToUpperToLowerAngle SurfaceRadiusAngle { get; } = SurfaceRadiusAngle;

    /// <inheritdoc />
    public ToUpperToLowerAngle AngleDistance { get; } = AngleDistance;

    /// <inheritdoc />
    public ToUpperToLower Volume { get; } = Volume;

    /// <inheritdoc />
    public NormalTangentialAngle SurfaceAngle { get; } = SurfaceAngle;

    /// <inheritdoc />
    public ToUpperToLower SurfaceEnergy { get; } = SurfaceEnergy;

    /// <inheritdoc />
    public ToUpperToLower SurfaceDiffusionCoefficient { get; } = SurfaceDiffusionCoefficient;

    /// <inheritdoc />
    public NormalTangential GibbsEnergyGradient { get; } = GibbsEnergyGradient;

    /// <inheritdoc />
    public NormalTangential VolumeGradient { get; } = VolumeGradient;
}