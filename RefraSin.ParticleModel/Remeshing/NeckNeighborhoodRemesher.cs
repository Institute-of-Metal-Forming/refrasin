using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(double deletionLimit = 0.3, double additionLimit = 3.0)
    : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle Remesh(IParticle particle)
    {
        var meanDiscretizationWidth = particle.Nodes.Average(n => n.SurfaceDistance.ToUpper);

        IEnumerable<IParticleNode> NodeFactory(IParticle newParticle) =>
            FilterNodes(
                newParticle,
                particle.Nodes,
                meanDiscretizationWidth * DeletionLimit,
                meanDiscretizationWidth * AdditionLimit
            );

        var newParticle = new Particle(
            particle.Id,
            particle.Coordinates,
            particle.RotationAngle,
            particle.MaterialId,
            NodeFactory
        );

        return newParticle;
    }

    private IEnumerable<IParticleNode> FilterNodes(
        IParticle particle,
        IEnumerable<IParticleNode> nodes,
        double minDistance,
        double maxDistance
    )
    {
        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (node.Upper.Type == Neck && node.SurfaceDistance.ToUpper < minDistance)
                    continue; // delete node
                if (node.Lower.Type == Neck && node.SurfaceDistance.ToLower < minDistance)
                    continue; // delete node
            }

            if (node.Type == GrainBoundary)
            {
                if (node.Lower.Type == Neck && node.SurfaceDistance.ToLower > maxDistance)
                {
                    yield return new ParticleNode(
                        Guid.NewGuid(),
                        particle,
                        node.Coordinates.PointHalfWayTo(node.Lower.Coordinates),
                        GrainBoundary
                    );
                }

                yield return new ParticleNode((INode)node, particle);

                if (node.Upper.Type == Neck && node.SurfaceDistance.ToUpper > maxDistance)
                {
                    yield return new ParticleNode(
                        Guid.NewGuid(),
                        particle,
                        node.Coordinates.PointHalfWayTo(node.Upper.Coordinates),
                        GrainBoundary
                    );
                }

                continue;
            }

            yield return new ParticleNode((INode)node, particle);
        }
    }

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;
}
