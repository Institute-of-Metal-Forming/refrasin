namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a node's times step.
/// </summary>
public record NodeTimeStep(
    Guid NodeId,
    double NormalDisplacement,
    double TangentialDisplacement,
    ToUpperToLower<double> DiffusionalFlow,
    double OuterDiffusionalFlow) : INodeTimeStep
{
    public NodeTimeStep(INodeTimeStep template) : this(
        template.NodeId,
        template.NormalDisplacement,
        template.TangentialDisplacement,
        template.DiffusionalFlow,
        template.OuterDiffusionalFlow
    ) { }

    /// <inheritdoc />
    public Guid NodeId { get; } = NodeId;

    /// <inheritdoc />
    public double NormalDisplacement { get; } = NormalDisplacement;

    /// <inheritdoc />
    public double TangentialDisplacement { get; } = TangentialDisplacement;

    /// <inheritdoc />
    public ToUpperToLower<double> DiffusionalFlow { get; } = DiffusionalFlow;

    /// <inheritdoc />
    public double OuterDiffusionalFlow { get; } = OuterDiffusionalFlow;
}