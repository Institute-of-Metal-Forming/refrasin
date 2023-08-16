using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Records;

/// <summary>
/// Represents an immutable record of a node's times step.
/// </summary>
/// <param name="NodeId"></param>
/// <param name="NormalDisplacement"></param>
/// <param name="TangentialDisplacement"></param>
/// <param name="DisplacementVector"></param>
/// <param name="DiffusionalFlow"></param>
/// <param name="OuterDiffusionalFlow"></param>
/// <param name="VolumeChange"></param>
public record NodeTimeStepRecord(
    Guid NodeId,
    double NormalDisplacement,
    double TangentialDisplacement,
    PolarVector DisplacementVector,
    ToUpperToLower DiffusionalFlow,
    double OuterDiffusionalFlow,
    double VolumeChange
) : INodeTimeStep
{
    public NodeTimeStepRecord(INodeTimeStep template) : this(
        template.NodeId,
        template.NormalDisplacement,
        template.TangentialDisplacement,
        template.DisplacementVector,
        template.DiffusionalFlow,
        template.OuterDiffusionalFlow,
        template.VolumeChange
    ) { }

    /// <inheritdoc />
    public Guid NodeId { get; } = NodeId;

    /// <inheritdoc />
    public double NormalDisplacement { get; } = NormalDisplacement;

    /// <inheritdoc />
    public double TangentialDisplacement { get; } = TangentialDisplacement;

    /// <inheritdoc />
    public PolarVector DisplacementVector { get; } = DisplacementVector;

    /// <inheritdoc />
    public ToUpperToLower DiffusionalFlow { get; } = DiffusionalFlow;

    /// <inheritdoc />
    public double OuterDiffusionalFlow { get; } = OuterDiffusionalFlow;

    /// <inheritdoc />
    public double VolumeChange { get; } = VolumeChange;
}