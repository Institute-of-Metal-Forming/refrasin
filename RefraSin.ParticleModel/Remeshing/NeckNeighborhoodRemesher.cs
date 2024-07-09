using RefraSin.Coordinates.Polar;
using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.ParticleModel.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(double deletionLimit = 0.2, double additionLimit = 3.0)
    : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle Remesh(IParticle particle)
    {
        var meanDiscretizationWidth = particle.Nodes.Average(n => n.SurfaceDistance.ToUpper);
        var nodes = AddNodesConditionally(
            DeleteNodesConditionally(
                particle.Nodes.ToArray(), meanDiscretizationWidth * DeletionLimit
            ), meanDiscretizationWidth * AdditionLimit
        );

        var newParticle = new Particle(
            particle.Id,
            particle.CenterCoordinates,
            particle.RotationAngle,
            particle.MaterialId,
            nodes.ToArray()
        );

        return newParticle;
    }

    private IEnumerable<INodeGeometry> DeleteNodesConditionally(IEnumerable<INodeGeometry> nodes, double minDistance)
    {
        foreach (var node in nodes)
        {
            if (node.Type == Surface)
            {
                if (node.Upper.Type == Neck && node.SurfaceDistance.ToUpper < minDistance)
                    continue;
                if (node.Lower.Type == Neck && node.SurfaceDistance.ToLower < minDistance)
                    continue;
            }

            yield return node;
        }
    }

    private IEnumerable<INodeGeometry> AddNodesConditionally(IEnumerable<INodeGeometry> nodes, double maxDistance)
    {
        foreach (var node in nodes)
        {
            if (node.Type == GrainBoundary)
            {
                if (node.Lower.Type == Neck && node.SurfaceDistance.ToLower > maxDistance)
                {
                    yield return new NodeGeometry(Guid.NewGuid(), node.Particle, node.Coordinates.PointHalfWayTo(node.Lower.Coordinates),
                        GrainBoundary);
                }

                yield return node;

                if (node.Upper.Type == Neck && node.SurfaceDistance.ToUpper > maxDistance)
                {
                    yield return new NodeGeometry(Guid.NewGuid(), node.Particle, node.Coordinates.PointHalfWayTo(node.Upper.Coordinates),
                        GrainBoundary);
                }

                continue;
            }

            yield return node;
        }
    }

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;
}