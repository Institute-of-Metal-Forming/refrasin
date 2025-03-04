using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;

namespace RefraSin.ParticleModel.ParticleFactories;

/// <summary>
/// Creates a particle with cosine ovality and cosine peaks.
/// </summary>
/// <param name="materialId">Material GUID</param>
/// <param name="centerCoordinates">coordinates of particle center</param>
/// <param name="rotationAngle">angle of particle rotation</param>
/// <param name="nodeCount">count of nodes to create</param>
/// <param name="baseRadius">base radius</param>
/// <param name="ovality">ovality as fraction of base radius (ge 0, lt 1)</param>
/// <param name="peakCount">count of peak waves (ge 0)</param>
/// <param name="peakHeight">height of peak waves as fraction of base radius (ge 0, lt 1)</param>
public class ShapeFunctionParticleFactoryCosOvalityCosPeaks(
    Guid materialId,
    AbsolutePoint centerCoordinates,
    Angle rotationAngle,
    int nodeCount,
    double baseRadius,
    double ovality = 0,
    int peakCount = 0,
    double peakHeight = 0
) : ShapeFunctionParticleFactory(materialId, centerCoordinates, rotationAngle, nodeCount)
{
    /// <inheritdoc />
    public override double ParticleShapeFunction(double phi) =>
        BaseRadius * (1 + PeakHeight * Math.Cos(PeakCount * phi) + Ovality * Math.Cos(2 * phi));

    public double BaseRadius { get; } =
        baseRadius > 0 ? baseRadius : throw new ArgumentOutOfRangeException(nameof(baseRadius));
    public double Ovality { get; } =
        ovality is >= 0 and < 1 ? ovality : throw new ArgumentOutOfRangeException(nameof(ovality));
    public int PeakCount { get; } =
        peakCount >= 0 ? peakCount : throw new ArgumentOutOfRangeException(nameof(peakCount));
    public double PeakHeight { get; } =
        peakHeight is >= 0 and < 1
            ? peakHeight
            : throw new ArgumentOutOfRangeException(nameof(peakHeight));
}
