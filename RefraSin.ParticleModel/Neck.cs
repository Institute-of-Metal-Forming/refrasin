namespace RefraSin.ParticleModel.Records;

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

    public INeckNode LowerNeckNode { get; } = LowerNeckNode;
    public INeckNode UpperNeckNode { get; } = UpperNeckNode;
    public IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes { get; } = GrainBoundaryNodes;
    public Guid ParticleId { get; } = ParticleId;
    public Guid ContactedParticleId { get; } = ContactedParticleId;
    public double Radius { get; } = Radius;
    public double GrainBoundaryLength { get; } = GrainBoundaryLength;

    public Guid Id { get; } = Id;
}