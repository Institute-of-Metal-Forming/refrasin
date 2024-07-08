using RefraSin.Coordinates.Polar;
using static System.Math;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.Remeshing;

public class FreeSurfaceRemesher(double deletionLimit = 0.05, double additionLimit = 0.5, double minWidthFactor = 0.25, double maxWidthFactor = 3.0)
    : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle Remesh(IParticle particle)
    {
        var meanDiscretizationWidth = particle.Nodes.Average(n => n.SurfaceDistance.ToUpper);
        var nodes = AddNodesConditionally(
            DeleteNodesConditionally(
                particle.Nodes.ToArray(), meanDiscretizationWidth * MaxWidthFactor
            ), meanDiscretizationWidth * MinWidthFactor
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

    private IEnumerable<INodeGeometry> DeleteNodesConditionally(IEnumerable<INodeGeometry> nodes, double maxDistance) => nodes.Where(n =>
        n.Type != NodeType.Surface || Abs(n.SurfaceRadiusAngle.Sum() - Pi) > DeletionLimit || n.SurfaceDistance.Sum() > maxDistance);

    private IEnumerable<INodeGeometry> AddNodesConditionally(IEnumerable<INodeGeometry> nodes, double minWidth)
    {
        var wasInsertedAtLastNode = true; // true to skip lower insertion on first node (will happen upper to the last)

        foreach (var node in nodes)
        {
            if (node.Type == NodeType.Surface && Abs(node.SurfaceRadiusAngle.Sum() - Pi) > AdditionLimit)
            {
                if (!wasInsertedAtLastNode && node.SurfaceDistance.ToLower > minWidth)
                    yield return new NodeGeometry(Guid.NewGuid(), node.Particle, node.Coordinates.PointHalfWayTo(node.Lower.Coordinates),
                        NodeType.Surface);
                yield return node;
                if (node.SurfaceDistance.ToUpper > minWidth)
                    yield return new NodeGeometry(Guid.NewGuid(), node.Particle, node.Coordinates.PointHalfWayTo(node.Upper.Coordinates),
                        NodeType.Surface);
                wasInsertedAtLastNode = true;
            }
            else
            {
                wasInsertedAtLastNode = false;
                yield return node;
            }
        }
    }

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;

    public double MinWidthFactor { get; } = minWidthFactor;

    public double MaxWidthFactor { get; } = maxWidthFactor;
}