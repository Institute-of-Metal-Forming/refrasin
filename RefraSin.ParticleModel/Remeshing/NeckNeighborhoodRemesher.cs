using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using Serilog;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(
    double localDeletionLimit = 0.5,
    double globalDeletionLimit = 0.3,
    double localAdditionLimit = 2.0,
    double globalAdditionLimit = 2.0
)
    : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle)
    {
        var logger = Log.ForContext<NeckNeighborhoodRemesher>();
        logger.Debug("Remeshing neck neighborhood of {Particle}.", particle);
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
                meanDiscretizationWidth * GlobalDeletionLimit,
                meanDiscretizationWidth * GlobalAdditionLimit,
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
        double maxDistance,
        ILogger logger
    )
    {
        var lastNodeDeleted = false;
        
        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (node.Lower.Type == Neck && node.Upper.Type != Neck)
                {
                    if (node.SurfaceDistance.ToLower < LocalDeletionLimit * node.SurfaceDistance.ToUpper || node.SurfaceDistance.ToLower < minDistance)
                    {
                        logger.Debug("Deleted node {Node}.", node);
                        lastNodeDeleted = true;
                        continue; // delete node
                    }

                    if (node.SurfaceDistance.ToLower > LocalAdditionLimit * node.SurfaceDistance.ToUpper || node.SurfaceDistance.ToLower > maxDistance)
                    {
                        var newNode = new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.Centroid(node.Lower.Coordinates),
                            Surface
                        );
                        logger.Debug("Added node {Node}.", newNode);
                        yield return newNode;
                        yield return new ParticleNode(node, particle);
                        continue;
                    }
                }
                
                if (node.Upper.Type == Neck && node.Lower.Type != Neck && !lastNodeDeleted)
                {
                    if (node.SurfaceDistance.ToUpper < LocalDeletionLimit * node.SurfaceDistance.ToLower || node.SurfaceDistance.ToUpper < minDistance)
                    {
                        logger.Debug("Deleted node {Node}.", node);
                        lastNodeDeleted = true;
                        continue; // delete node
                    }

                    if (node.SurfaceDistance.ToUpper > LocalAdditionLimit * node.SurfaceDistance.ToLower || node.SurfaceDistance.ToUpper > maxDistance)
                    {
                        var newNode = new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.Centroid(node.Upper.Coordinates),
                            Surface
                        );
                        logger.Debug("Added node {Node}.", newNode);
                        yield return new ParticleNode(node, particle);
                        yield return newNode;
                        continue;
                    }
                }
            }

            yield return new ParticleNode(node, particle);
            lastNodeDeleted = false;
        }
    }

    public double LocalDeletionLimit { get; } = localDeletionLimit;
    public double GlobalDeletionLimit { get; } = globalDeletionLimit;
    public double LocalAdditionLimit { get; } = localAdditionLimit;
    public double GlobalAdditionLimit { get; } = globalAdditionLimit;
}
