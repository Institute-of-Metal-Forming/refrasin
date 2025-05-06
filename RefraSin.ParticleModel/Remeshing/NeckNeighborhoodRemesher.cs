using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using Serilog;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(double deletionLimit = 0.5) : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle)
    {
        var logger = Log.ForContext<NeckNeighborhoodRemesher>();
        logger.Debug("Remeshing particle {Particle}.", particle);
        var meanDiscretizationWidth =
            particle.Nodes.Where(n => n.Type == Surface).Average(n => n.SurfaceDistance.Sum) / 2;
        logger.Debug(
            "Reference discretization width is {DiscretizationWidth}",
            meanDiscretizationWidth
        );

        IEnumerable<IParticleNode> NodeFactory(IParticle<IParticleNode> newParticle) =>
            FilterNodes(
                newParticle,
                particle.Nodes,
                meanDiscretizationWidth * DeletionLimit,
                logger
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
        double minDistance,
        ILogger logger
    )
    {
        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (
                    node.Upper.Type == Neck
                    && node.Lower.Type != Neck
                    && node.SurfaceDistance.ToUpper < minDistance
                )
                {
                    logger.Debug("Deleted node {Node}.", node);
                    continue; // delete node
                }

                if (
                    node.Lower.Type == Neck
                    && node.Upper.Type != Neck
                    && node.SurfaceDistance.ToLower < minDistance
                )
                {
                    logger.Debug("Deleted node {Node}.", node);
                    continue; // delete node
                }
            }

            yield return new ParticleNode(node, particle);
        }
    }

    public double DeletionLimit { get; } = deletionLimit;
}
