using RefraSin.Coordinates.Polar;
using static System.Math;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.Remeshing;

public class NeckNeighborhoodRemesher(double deletionLimit = 0.2, double additionLimit = 2.5)
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
        foreach (var n in nodes)
        {
            if (n.Type == NodeType.Surface)
            {
                if (n.Upper.Type == NodeType.Neck && n.SurfaceDistance.ToUpper < minDistance)
                    continue;
                if (n.Lower.Type == NodeType.Neck && n.SurfaceDistance.ToLower < minDistance)
                    continue;
            }

            yield return n;
        }
    }

    private IEnumerable<INodeGeometry> AddNodesConditionally(IEnumerable<INodeGeometry> nodes, double minWidth)
        => nodes;

    public double DeletionLimit { get; } = deletionLimit;

    public double AdditionLimit { get; } = additionLimit;
}