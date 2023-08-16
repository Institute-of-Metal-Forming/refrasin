using RefraSin.ParticleModel;

namespace RefraSin.Storage.ParticleModel;

public class NeckState : INeck
{
    public NeckState(INeck template)
    {
        LowerNeckNode = new NeckNodeRecord(template.LowerNeckNode);
        UpperNeckNode = new NeckNodeRecord(template.UpperNeckNode);
        GrainBoundaryNodes = template.GrainBoundaryNodes.Select(n => new GrainBoundaryNodeRecord(n)).ToArray();
        ParticleId = template.ParticleId;
        ContactedParticleId = template.ContactedParticleId;
        Radius = template.Radius;
        GrainBoundaryLength = template.GrainBoundaryLength;
        Id = template.Id;
    }

    public INeckNode LowerNeckNode { get; }
    public INeckNode UpperNeckNode { get; }
    public IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes { get; }
    public Guid ParticleId { get; }
    public Guid ContactedParticleId { get; }
    public double Radius { get; }
    public double GrainBoundaryLength { get; }

    public int Id { get; }
}