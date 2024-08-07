using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Polar;

/// <summary>
///     Stellt einen Punkt in Polarkoordinaten dar.
/// </summary>
public readonly struct PolarPoint : IPolarPoint, IPointArithmetics<PolarPoint, PolarVector>
{
    /// <summary>
    ///     Creates the point (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarPoint(IPolarCoordinateSystem? system = null)
    {
        Phi = 0;
        R = 0;
        System = system ?? PolarCoordinateSystem.Default;
    }

    /// <summary>
    ///     Creates the point (phi, r).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarPoint((Angle phi, double r) coordinates, IPolarCoordinateSystem? system = null)
        : this(system)
    {
        (Phi, R) = coordinates;
    }

    /// <summary>
    ///     Creates the point (phi, r).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="phi">angle coordinate</param>
    /// <param name="r">radius coordinate</param>
    public PolarPoint(Angle phi, double r, IPolarCoordinateSystem? system = null)
        : this(system)
    {
        Phi = phi;
        R = r;
    }

    /// <summary>
    ///     Creates a point based on a template. The coordinates systems are automatically cast.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarPoint(IPoint other, IPolarCoordinateSystem? system = null)
        : this(system)
    {
        var absoluteCoordinates = other.Absolute;
        var originCoordinates = System.Origin.Absolute;
        var x = absoluteCoordinates.X - originCoordinates.X;
        var y = absoluteCoordinates.Y - originCoordinates.Y;
        R = Sqrt(Pow(x, 2) + Pow(y, 2)) / System.RScale;
        Phi =
            (
                y > 0
                    ? Acos(x / R)
                    : y < 0
                        ? PI + Acos(-x / R)
                        : x >= 0
                            ? 0
                            : PI
            ) - System.RotationAngle;
    }

    /// <inheritdoc />
    public Angle Phi { get; }

    /// <inheritdoc />
    public double R { get; }

    /// <inheritdoc />
    public IPolarCoordinateSystem System { get; }

    /// <inheritdoc />
    public Angle AngleTo(IPolarCoordinates other, bool allowNegative = false)
    {
        if (System.Equals(other.System))
        {
            var diff = (other.Phi - Phi).Reduce(Angle.ReductionDomain.WithNegative);
            if (allowNegative)
                return diff;
            return Abs(diff);
        }

        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <inheritdoc />
    public AbsolutePoint Absolute
    {
        get
        {
            var origin = System.Origin.Absolute;
            return new AbsolutePoint(
                origin.X + R * System.RScale * Cos(Phi + System.RotationAngle),
                origin.Y + R * System.RScale * Sin(Phi + System.RotationAngle)
            );
        }
    }

    /// <inheritdoc />
    public PolarPoint AddVector(PolarVector v) => this + v;

    /// <inheritdoc />
    public PolarVector VectorTo(PolarPoint p) => p - this;

    /// <inheritdoc />
    public double DistanceTo(PolarPoint other)
    {
        if (System.Equals(other.System))
            return CosLaw.C(R, other.R, Abs(other.Phi - Phi));
        return Absolute.DistanceTo(other.Absolute);
    }

    /// <inheritdoc />
    public bool IsClose(IPolarPoint other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return R.IsClose(other.R, precision) && Phi.IsClose(other.Phi, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    public PolarPoint Centroid(PolarPoint other)
    {
        if (System.Equals(other.System))
        {
            var angle = (other.Phi - Phi).Reduce(Angle.ReductionDomain.WithNegative);
            var dist = CosLaw.C(R, other.R, Abs(angle));
            var s = Sqrt(2 * (Pow(R, 2) + Pow(other.R, 2)) - Pow(dist, 2)) / 2;
            return new PolarPoint(Phi + Sign(angle) * CosLaw.Gamma(R, s, dist / 2), s, System);
        }

        return new PolarPoint(Absolute.Centroid(other.Absolute), System);
    }

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="PolarCoordinates.ToString(string, string, string, IFormatProvider)" />
    /// </remarks>
    public static PolarPoint Parse(string s)
    {
        var (value1, value2) = s.ParseCoordinateString(nameof(PolarPoint));
        return new PolarPoint(Angle.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarPoint operator +(PolarPoint p, PolarVector v)
    {
        if (p.System.Equals(v.System))
        {
            var x = p.R * Cos(p.Phi) + v.R * Cos(v.Phi);
            var y = p.R * Sin(p.Phi) + v.R * Sin(v.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi =
                y > 0
                    ? Acos(x / r)
                    : y < 0
                        ? PI + Acos(-x / r)
                        : x >= 0
                            ? 0
                            : PI;

            return new PolarPoint(phi, r, p.System);
        }

        throw new DifferentCoordinateSystemException(p, v);
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    public static PolarPoint operator +(PolarVector v, PolarPoint p) => p + v;

    /// <summary>
    ///     Subtraction of a vector from a point, see <see cref="AddVector" />.
    /// </summary>
    public static PolarPoint operator -(PolarPoint p, PolarVector v) => p + -v;

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator -(PolarPoint p1, PolarPoint p2)
    {
        if (p1.System.Equals(p2.System))
        {
            var x = p1.R * Cos(p1.Phi) - p2.R * Cos(p2.Phi);
            var y = p1.R * Sin(p1.Phi) - p2.R * Sin(p2.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi =
                y > 0
                    ? Acos(x / r)
                    : y < 0
                        ? PI + Acos(-x / r)
                        : x >= 0
                            ? 0
                            : PI;

            return new PolarVector(phi, r, p1.System);
        }

        throw new DifferentCoordinateSystemException(p1, p2);
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatPolarCoordinates(format, formatProvider);

    /// <inheritdoc />
    public double[] ToArray() => [Phi, R];

    /// <inheritdoc />
    public static PolarPoint operator -(PolarPoint value) =>
        new(value.Phi + Angle.Straight, value.R);

    /// <inheritdoc />
    public PolarPoint ScaleBy(double scale) => new(Phi, scale * R);

    /// <inheritdoc />
    public PolarPoint RotateBy(double rotation) => new(Phi + rotation, R);
}
