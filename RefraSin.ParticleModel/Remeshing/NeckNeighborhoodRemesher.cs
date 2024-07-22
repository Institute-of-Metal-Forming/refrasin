using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(double deletionLimit = 0.3)
    : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle)
    {
        var meanDiscretizationWidth = particle.Nodes.Average(n => n.SurfaceDistance.ToUpper);

        IEnumerable<IParticleNode> NodeFactory(IParticle<IParticleNode> newParticle) =>
            FilterNodes(
                newParticle,
                particle.Nodes,
                meanDiscretizationWidth * DeletionLimit
            );

        var newParticle = new Particle<IParticleNode>(
            particle.Id,
            particle.Coordinates,
            particle.RotationAngle,
            particle.MaterialId,
            NodeFactory
        );

        return newParticle;
    }

    private IEnumerable<IParticleNode> FilterNodes(
        IParticle<IParticleNode> particle,
        IEnumerable<IParticleNode> nodes,
        double minDistance
    )
    {
        foreach (var node in nodes)
        {
            if (node.Upper.Type == Neck && node.SurfaceDistance.ToUpper < minDistance)
                continue; // delete node
            if (node.Lower.Type == Neck && node.SurfaceDistance.ToLower < minDistance)
                continue; // delete node

            yield return new ParticleNode(node, particle);
        }
    }

    public double DeletionLimit { get; } = deletionLimit;
}