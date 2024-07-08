using RefraSin.Coordinates.Polar;
using static System.Math;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.Remeshing;

public class FreeSurfaceRemesher(double deletionLimit = 0.05, double additionLimit = 0.3) : IParticleRemesher
{
    /// <inheritdoc />
    public IParticle Remesh(IParticle particle)
    {
        var nodes = AddNodesConditionally(
            DeleteNodesConditionally(
                particle.Nodes
            )
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

    private IEnumerable<INodeGeometry> DeleteNodesConditionally(IEnumerable<INodeGeometry> nodes) =>
        nodes.Where(
            n =>
                n.Type != NodeType.Surface || Abs(n.SurfaceRadiusAngle.Sum() - Pi) > DeletionLimit
        );

    private IEnumerable<INodeGeometry> AddNodesConditionally(IEnumerable<INodeGeometry> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.Type == NodeType.Surface && Abs(node.SurfaceRadiusAngle.Sum() - Pi) > AdditionLimit)
            {
                // var phil = -node.AngleDistance.ToLower.Radians;
                // var phiu = node.AngleDistance.ToUpper.Radians;
                //
                // var a = phil * (node.Coordinates.R - node.Upper.Coordinates.R) + phiu * (node.Lower.Coordinates.R - node.Coordinates.R);
                // var b = Pow(phil, 2) * (node.Coordinates.R - node.Upper.Coordinates.R) +
                //         Pow(phiu, 2) * (node.Lower.Coordinates.R - node.Coordinates.R);
                // var d = phil * phiu * (phil - phiu);
                //
                // double Interpolation(double phi) => a / d * Pow(phi, 2) - b / d * phi + node.Coordinates.R;
                //
                // var lowerAdditionCoordinates = new PolarPoint((node.Lower.Coordinates.Phi + node.Coordinates.Phi) / 2, Interpolation(phil / 2));
                // var upperAdditionCoordinates = new PolarPoint((node.Upper.Coordinates.Phi + node.Coordinates.Phi) / 2, Interpolation(phiu / 2));
                //
                // yield return new NodeGeometry(Guid.NewGuid(), node.Particle, lowerAdditionCoordinates, NodeType.Surface);
                // yield return node;
                // yield return new NodeGeometry(Guid.NewGuid(), node.Particle, upperAdditionCoordinates, NodeType.Surface);
                
                yield return new NodeGeometry(Guid.NewGuid(), node.Particle, node.Coordinates.PointHalfWayTo(node.Lower.Coordinates), NodeType.Surface);
                yield return node;
                yield return new NodeGeometry(Guid.NewGuid(), node.Particle, node.Coordinates.PointHalfWayTo(node.Upper.Coordinates), NodeType.Surface);
            }
            else
                yield return node;
        }
    }

    public double DeletionLimit { get; } = deletionLimit;
    public double AdditionLimit { get; } = additionLimit;
}