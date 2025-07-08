using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using Serilog;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(double deletionLimit = 0.5, double additionLimit = 2.0) : IParticleRemesher
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
                meanDiscretizationWidth * AdditionLimit,
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
        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (
                    node.Upper.Type == Neck
                )
                {
                    if (node.SurfaceDistance.ToUpper < minDistance)
                    {
                        if (node.Lower.Type == Neck)
                            throw new InvalidOperationException("Last surface node to be removed.");

                        logger.Debug("Deleted node {Node}.", node);
                        continue; // delete node
                    }

                    if (node.SurfaceDistance.ToUpper > maxDistance)
                    {
                        var newNode = new ParticleNode(Guid.NewGuid(), particle, node.Coordinates.Centroid(node.Upper.Coordinates), Surface);
                        logger.Debug("Added node {Node}.", newNode);
                        yield return new ParticleNode(node, particle);
                        yield return newNode;
                        continue;
                    }
                }

                if (
                    node.Lower.Type == Neck
                )
                {
                    if (node.SurfaceDistance.ToLower < minDistance)
                    {
                        if (node.Upper.Type == Neck)
                            throw new InvalidOperationException("Last surface node to be removed.");

                        logger.Debug("Deleted node {Node}.", node);
                        continue; // delete node
                    }

                    if (node.SurfaceDistance.ToLower > maxDistance)
                    {
                        var newNode = new ParticleNode(Guid.NewGuid(), particle, node.Coordinates.Centroid(node.Lower.Coordinates), Surface);
                        logger.Debug("Added node {Node}.", newNode);
                        yield return newNode;
                        yield return new ParticleNode(node, particle);
                        continue;
                    }
                }
            }

            yield return new ParticleNode(node, particle);
        }
    }

    public double DeletionLimit { get; } = deletionLimit;
    public double AdditionLimit { get; } = additionLimit;
}
