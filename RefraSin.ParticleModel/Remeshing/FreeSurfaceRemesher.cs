using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using Serilog;
using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.ParticleModel.Nodes.NodeType;
using Log = Serilog.Log;

namespace RefraSin.ParticleModel.Remeshing;

public class FreeSurfaceRemesher(
    double deletionLimit = 0.05,
    double additionLimit = double.PositiveInfinity,
    double minWidthFactor = 0.25,
    double maxWidthFactor = 3.0,
    double twinPointLimit = 0.1,
    double neckProtectionCount = 10
) : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle)
    {
        var logger = Log.ForContext<FreeSurfaceRemesher>();
        logger.Debug("Remeshing free surface of {Particle}.", particle);
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
                meanDiscretizationWidth * MinWidthFactor,
                meanDiscretizationWidth * MaxWidthFactor,
                meanDiscretizationWidth * TwinPointLimit,
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
        double twinPointDistance,
        ILogger logger
    )
    {
        var wasInsertedAtLastNode = true; // true to skip lower insertion on first node (will happen upper to the last)
        var lastNodeDeleted = false;
        IParticleNode? lowerTwin = null;

        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (lowerTwin is not null)
                {
                    var combined = new ParticleNode(
                        Guid.NewGuid(),
                        particle,
                        node.Coordinates.Centroid(lowerTwin.Coordinates),
                        Surface
                    );
                    yield return combined;
                    logger.Debug(
                        "Combined nearby twin of {First} and {Second} to {Combined}.",
                        lowerTwin,
                        node,
                        combined
                    );

                    lowerTwin = null;
                    continue;
                }

                if (node.SurfaceDistance.ToUpper < twinPointDistance && node.Upper.Type == node.Type)
                {
                    lowerTwin = node;
                    continue;
                }

                if (
                    !lastNodeDeleted
                    && Abs(node.SurfaceRadiusAngle.Sum - Pi) < DeletionLimit
                    && node.SurfaceDistance.Sum < maxDistance
                    && node.Upper.Type != Neck
                    && node.Lower.Type != Neck
                    && !IsNodeInProtection(node)
                )
                {
                    logger.Debug("Deleted node {Node}.", node);
                    lastNodeDeleted = true;
                    continue; // delete node
                }

                lastNodeDeleted = false;

                if (Abs(node.SurfaceRadiusAngle.Sum - Pi) > AdditionLimit)
                {
                    if (!wasInsertedAtLastNode && node.SurfaceDistance.ToLower > minDistance)
                    {
                        var added = new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.Centroid(node.Lower.Coordinates),
                            Surface
                        );
                        yield return added;
                        logger.Debug("Added node {Added} below {Present}.", added, node);
                    }

                    yield return new ParticleNode(node, particle); // existing node
                    if (node.SurfaceDistance.ToUpper > minDistance)
                    {
                        var added = new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.Centroid(node.Upper.Coordinates),
                            Surface
                        );
                        yield return added;
                        logger.Debug("Added node {Added} above {Present}.", added, node);
                        wasInsertedAtLastNode = true;
                    }
                    else
                        wasInsertedAtLastNode = false;

                    continue;
                }
            }

            lowerTwin = null;
            wasInsertedAtLastNode = false;
            yield return new ParticleNode(node, particle); // keep node
        }
    }

    private bool IsNodeInProtection(IParticleNode node)
    {
        {
            var currentNode = node;
            for (int i = 0; i < NeckProtectionCount; i++)
            {
                currentNode = currentNode.Upper;
                if (currentNode.Type == Neck)
                    return true;
            }
        }
        {
            var currentNode = node;
            for (int i = 0; i < NeckProtectionCount; i++)
            {
                currentNode = currentNode.Lower;
                if (currentNode.Type == Neck)
                    return true;
            }
        }
        return false;
    }

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;

    public double MinWidthFactor { get; } = minWidthFactor;

    public double MaxWidthFactor { get; } = maxWidthFactor;

    public double TwinPointLimit { get; } = twinPointLimit;

    public double NeckProtectionCount { get; } = neckProtectionCount;
}
