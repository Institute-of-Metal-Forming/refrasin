using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.ParticleFactories;

public abstract class ShapeFunctionParticleFactory(
    Guid materialId,
    AbsolutePoint centerCoordinates,
    Angle rotationAngle,
    int nodeCount
) : IParticleFactory<Particle<ParticleNode>, ParticleNode>
{
    public Guid MaterialId { get; } = materialId;

    public AbsolutePoint CenterCoordinates { get; } = centerCoordinates;

    public Angle RotationAngle { get; } = rotationAngle;

    public int NodeCount { get; } = nodeCount;

    public abstract double ParticleShapeFunction(double phi);

    /// <inheritdoc />
    public Particle<ParticleNode> GetParticle(Guid? id = null)
    {
        var phis = Enumerable.Range(0, NodeCount).Select(i => TwoPi * i / NodeCount).ToArray();
        var rs = phis.Select(ParticleShapeFunction).ToArray();

        return new Particle<ParticleNode>(
            id ?? Guid.NewGuid(),
            CenterCoordinates,
            RotationAngle,
            MaterialId,
            NodeFactory
        );

        IEnumerable<ParticleNode> NodeFactory(IParticle<ParticleNode> particle) =>
            phis.Zip(rs)
                .Select(t => new ParticleNode(
                    Guid.NewGuid(),
                    particle,
                    new PolarPoint(t.First, t.Second),
                    NodeType.Surface
                ));
    }
}
