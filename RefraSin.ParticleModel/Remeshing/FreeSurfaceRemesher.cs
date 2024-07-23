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
    double maxWidthFactor = 3.0
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
                meanDiscretizationWidth * MaxWidthFactor
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
        double maxDistance
    )
    {
        var wasInsertedAtLastNode = true; // true to skip lower insertion on first node (will happen upper to the last)

        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (
                    node.Upper.Type != Neck
                    && node.Lower.Type != Neck
                    && Abs(node.SurfaceRadiusAngle.Sum - Pi) < DeletionLimit
                    && node.SurfaceDistance.Sum < maxDistance
                )
                    continue; // delete node

                if (Abs(node.SurfaceRadiusAngle.Sum - Pi) > AdditionLimit)
                {
                    if (!wasInsertedAtLastNode && node.SurfaceDistance.ToLower > minDistance)
                        yield return new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.PointHalfWayTo(node.Lower.Coordinates),
                            Surface
                        );
                    yield return new ParticleNode((INode)node, particle);
                    if (node.SurfaceDistance.ToUpper > minDistance)
                        yield return new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            node.Coordinates.PointHalfWayTo(node.Upper.Coordinates),
                            Surface
                        );
                    wasInsertedAtLastNode = true;
                    continue;
                }
            }

            wasInsertedAtLastNode = false;
            yield return new ParticleNode((INode)node, particle);
        }
    }

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;

    public double MinWidthFactor { get; } = minWidthFactor;

    public double MaxWidthFactor { get; } = maxWidthFactor;
}
