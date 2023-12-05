using MathNet.Numerics;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using static MathNet.Numerics.Constants;
using static System.Math;

namespace RefraSin.ParticleModel.ParticleFactories;

public class ShapeFunctionParticleFactory : IParticleFactory
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

    public AbsolutePoint CenterCoordinates { get; set; } = new();

    public Angle RotationAngle { get; } = 0;

    public int NodeCount { get; set; } = 100;

    public Func<ShapeFunctionParticleFactory, int>? NodeCountFunction { get; set; }

    public virtual double ParticleShapeFunction(double phi) => BaseRadius * (1 + PeakHeight * Cos(PeakCount * phi) + Ovality * Cos(2 * phi));

    /// <inheritdoc />
    public IParticle GetParticle()
    {
        var nodeCount = NodeCountFunction?.Invoke(this) ?? NodeCount;

        var phis = Generate.LinearSpaced(nodeCount + 1, 0, Pi2)[0..^1];
        var rs = Generate.Map(phis, ParticleShapeFunction);
        var particleId = Guid.NewGuid();

        return new Particle(
            particleId,
            CenterCoordinates,
            RotationAngle,
            MaterialId,
            Generate.Map2(phis, rs,
                (phi, r) => new Node(
                    Guid.NewGuid(),
                    particleId,
                    new PolarPoint(phi, r)
                )
            )
        );
    }
}