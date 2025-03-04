using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class FreeSurfaceRemesher(
    double deletionLimit = 0.05,
    double additionLimit = 0.5,
    double minWidthFactor = 0.25,
    double maxWidthFactor = 3.0,
    double twinPointLimit = 0.1
) : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle)
    {
        var meanDiscretizationWidth = particle.Nodes.Average(n => n.SurfaceDistance.ToUpper);

        IEnumerable<IParticleNode> NodeFactory(IParticle<IParticleNode> newParticle) =>
            FilterNodes(
                newParticle,
                particle.Nodes,
                meanDiscretizationWidth * MinWidthFactor,
                meanDiscretizationWidth * MaxWidthFactor,
                meanDiscretizationWidth * TwinPointLimit
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
        double twinPointDistance
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
                    yield return new ParticleNode(
                        Guid.NewGuid(),
                        particle,
                        node.Coordinates.Centroid(lowerTwin.Coordinates),
                        Surface
                    );
                    lowerTwin = null;
                    continue;
                }

                if (node.SurfaceDistance.ToUpper < twinPointDistance)
                {
                    lowerTwin = node;
                    continue;
                }

                if (
                    !lastNodeDeleted
                    && node.Upper.Type != Neck
                    && node.Lower.Type != Neck
                    && Abs(node.SurfaceRadiusAngle.Sum - Pi) < DeletionLimit
                    && node.SurfaceDistance.Sum < maxDistance
                )
                {
                    lastNodeDeleted = true;
                    continue; // delete node
                }

                lastNodeDeleted = false;

                if (Abs(node.SurfaceRadiusAngle.Sum - Pi) > AdditionLimit)
                {
                    if (!wasInsertedAtLastNode && node.SurfaceDistance.ToLower > minDistance)
                        yield return new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.Centroid(node.Lower.Coordinates),
                            Surface
                        );
                    yield return new ParticleNode(node, particle); // existing node
                    if (node.SurfaceDistance.ToUpper > minDistance)
                    {
                        yield return new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.Centroid(node.Upper.Coordinates),
                            Surface
                        );
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

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;

    public double MinWidthFactor { get; } = minWidthFactor;

    public double MaxWidthFactor { get; } = maxWidthFactor;

    public double TwinPointLimit { get; } = twinPointLimit;
}
