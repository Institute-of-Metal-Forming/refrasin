using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using static System.Math;

namespace RefraSin.ParticleModel.ParticleFactories;

/// <summary>
/// Creates a particle with ellipse ovality and cosine peaks.
/// </summary>
/// <param name="materialId">Material GUID</param>
/// <param name="centerCoordinates">coordinates of particle center</param>
/// <param name="rotationAngle">angle of particle rotation</param>
/// <param name="nodeCount">count of nodes to create</param>
/// <param name="baseRadius">base radius</param>
/// <param name="ovality">ovality as the ratio between small and large diameter (ge 1)</param>
/// <param name="peakCount">count of peak waves (ge 0)</param>
/// <param name="peakHeight">height of peak waves as fraction of base radius (ge 0, lt 1)</param>
public class ShapeFunctionParticleFactoryEllipseOvalityCosPeaks(
    Guid materialId,
    AbsolutePoint centerCoordinates,
    Angle rotationAngle,
    int nodeCount,
    double baseRadius,
    double ovality = 1,
    int peakCount = 0,
    double peakHeight = 0,
    double peakShift = 0
) : ShapeFunctionParticleFactory(materialId, centerCoordinates, rotationAngle, nodeCount)
{
    private double EllipseFunction(double phi)
    {
        var factor = Sqrt(Ovality);
        var a = factor * BaseRadius;
        var b = BaseRadius / factor;
        return a * b / Sqrt(Pow(a * Sin(phi), 2) + Pow(b * Cos(phi), 2));
    }

    /// <inheritdoc />
    public override double ParticleShapeFunction(double phi) =>
        EllipseFunction(phi) + BaseRadius * PeakHeight * Cos(PeakCount * phi + 2 * PI * PeakShift);

    public double BaseRadius { get; } =
        baseRadius > 0 ? baseRadius : throw new ArgumentOutOfRangeException(nameof(baseRadius));
    public double Ovality { get; } =
        ovality >= 1 ? ovality : throw new ArgumentOutOfRangeException(nameof(ovality));
    public int PeakCount { get; } =
        peakCount >= 0 ? peakCount : throw new ArgumentOutOfRangeException(nameof(peakCount));
    public double PeakHeight { get; } =
        peakHeight is >= 0 and < 1
            ? peakHeight
            : throw new ArgumentOutOfRangeException(nameof(peakHeight));
    public double PeakShift { get; } =
        peakShift is >= 0 and < 0.5
            ? peakShift
            : throw new ArgumentOutOfRangeException(nameof(peakHeight));
}
