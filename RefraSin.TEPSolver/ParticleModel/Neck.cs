using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

public class Neck : INeck
{
    public Neck(NeckNode lower)
    {
        ParticleId = lower.ParticleId;
        ContactedParticleId = lower.ContactedParticleId;
        LowerNeckNode = lower;
        GrainBoundaryLength = lower.SurfaceDistance.ToUpper;
        var grainBoundaryNodes = new List<GrainBoundaryNode>();
        var next = lower.Upper;

        while (next is GrainBoundaryNode grainBoundaryNode)
        {
            grainBoundaryNodes.Add(grainBoundaryNode);
            GrainBoundaryLength += grainBoundaryNode.SurfaceDistance.ToUpper;
            next = next.Upper;
        }

        GrainBoundaryNodes = grainBoundaryNodes;
        var upper = (NeckNode)next;
        UpperNeckNode = upper;
        Radius = (UpperNeckNode.AbsoluteCoordinates - LowerNeckNode.AbsoluteCoordinates).Norm / 2;
        Id = Guid.NewGuid();
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public INeckNode LowerNeckNode { get; }

    /// <inheritdoc />
    public INeckNode UpperNeckNode { get; }

    /// <inheritdoc />
    public IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes { get; }

    /// <inheritdoc />
    public Guid ParticleId { get; }

    /// <inheritdoc />
    public Guid ContactedParticleId { get; }

    /// <inheritdoc />
    public double Radius { get; }

    /// <inheritdoc />
    public double GrainBoundaryLength { get; }

    /// <summary>
    /// Generates a hash where order of supplied Ids doesn't matter.
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    private int MergeGuidsToHash(params Guid[] ids)
    {
        var hashes = ids.Select(i => i.GetHashCode()).OrderBy(i => i).ToArray();
        var hash = new HashCode();
        foreach (var h in hashes)
        {
            hash.Add(h);
        }

        return hash.ToHashCode();
    }
}