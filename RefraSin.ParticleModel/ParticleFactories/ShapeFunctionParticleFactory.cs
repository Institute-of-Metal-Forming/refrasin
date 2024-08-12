using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using static System.Math;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.ParticleFactories;

public class ShapeFunctionParticleFactory : IParticleFactory<Particle<ParticleNode>, ParticleNode>
{
    public ShapeFunctionParticleFactory(double baseRadius, double peakHeight, uint peakCount, double ovality, Guid materialId)
    {
        BaseRadius = baseRadius;
        PeakHeight = peakHeight;
        PeakCount = peakCount;
        Ovality = ovality;
        MaterialId = materialId;
    }

    public double BaseRadius { get; }

    public double PeakHeight { get; }

    public uint PeakCount { get; }

    public double Ovality { get; }

    public Guid MaterialId { get; }

    public AbsolutePoint CenterCoordinates { get; init; } = new();

    public Angle RotationAngle { get; init; } = 0;

    public int NodeCount { get; init; } = 100;

    public Func<ShapeFunctionParticleFactory, int>? NodeCountFunction { get; init; }

    public virtual double ParticleShapeFunction(double phi) => BaseRadius * (1 + PeakHeight * Cos(PeakCount * phi) + Ovality * Cos(2 * phi));

    /// <inheritdoc />
    public Particle<ParticleNode> GetParticle(Guid? id = null)
    {
        var nodeCount = NodeCountFunction?.Invoke(this) ?? NodeCount;

        var phis = Enumerable.Range(0, nodeCount).Select(i => TwoPi * i / nodeCount).ToArray();
        var rs = phis.Select(ParticleShapeFunction).ToArray();

        return new Particle<ParticleNode>(
            id ?? Guid.NewGuid(),
            CenterCoordinates,
            RotationAngle,
            MaterialId,
            NodeFactory
        );

        IEnumerable<ParticleNode> NodeFactory(IParticle<ParticleNode> particle) =>
            phis.Zip(rs).Select(
                t => new ParticleNode(
                    Guid.NewGuid(),
                    particle,
                    new PolarPoint(t.First, t.Second),
                    NodeType.Surface
                )
            );
    }
}