using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Represents a cartesian coordinate system.
/// </summary>
public class CartesianCoordinateSystem : CoordinateSystem, ICloneable<CartesianCoordinateSystem>
{
    /// <summary>
    ///     Creates a standard cartesian system: <see cref="CoordinateSystem.Origin" /> in (0,0),
    ///     <see cref="CoordinateSystem.RotationAngle" /> = 0.
    /// </summary>
    public CartesianCoordinateSystem() { }

    /// <summary>
    ///     Creates a new cartesian system.
    /// </summary>
    /// <param name="origin">point of origin, if null Absolute(0, 0) is used</param>
    /// <param name="rotationAngle">rotation angle</param>
    /// <param name="xScale">scale along X</param>
    /// <param name="yScale">scale along Y</param>
    public CartesianCoordinateSystem(IPoint? origin, double rotationAngle = 0, double xScale = 1,
        double yScale = 1)
        : base(origin, rotationAngle)
    {
        XScale = xScale;
        YScale = yScale;
    }

    /// <summary>
    ///     Default instance.
    /// </summary>
    public static CartesianCoordinateSystem Default => Default<CartesianCoordinateSystem>.Instance;

    /// <summary>
    ///     Gets or sets the scale along X axis.
    /// </summary>
    public double XScale { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the scale along Y axis.
    /// </summary>
    public double YScale { get; set; } = 1;

    /// <inheritdoc />
    public CartesianCoordinateSystem Clone() => new(Origin, RotationAngle, XScale, YScale);

    /// <inheritdoc />
    public override string ToString() => $"{nameof(CartesianCoordinateSystem)} {Label}".Trim();
}