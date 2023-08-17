namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a neck.
/// </summary>
public record Neck(
    Guid Id,
    Guid ParticleId,
    Guid ContactedParticleId,
    INeckNode LowerNeckNode,
    INeckNode UpperNeckNode,
    IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes,
    double Radius,
    double GrainBoundaryLength
) : INeck
{
    public Neck(INeck template) : this(
        template.Id,
        template.ParticleId,
        template.ContactedParticleId,
        new NeckNode(template.LowerNeckNode),
        new NeckNode(template.UpperNeckNode),
        template.GrainBoundaryNodes.Select(n => new GrainBoundaryNode(n)).ToArray(),
        template.Radius,
        template.GrainBoundaryLength
    ) { }

    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public INeckNode LowerNeckNode { get; } = LowerNeckNode;

    /// <inheritdoc />
    public INeckNode UpperNeckNode { get; } = UpperNeckNode;

    /// <inheritdoc />
    public IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes { get; } = GrainBoundaryNodes;

    /// <inheritdoc />
    public Guid ParticleId { get; } = ParticleId;

    /// <inheritdoc />
    public Guid ContactedParticleId { get; } = ContactedParticleId;

    /// <inheritdoc />
    public double Radius { get; } = Radius;

    /// <inheritdoc />
    public double GrainBoundaryLength { get; } = GrainBoundaryLength;
}