using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Polar;

public readonly struct PolarPoint(Angle phi, double r, IPolarCoordinateSystem? system = null)
    : IPolarPoint,
        IPointArithmetics<PolarPoint, PolarVector>
{
    public PolarPoint(IPolarCoordinateSystem? system = null)
        : this(0, 0, system) { }

    public PolarPoint((Angle phi, double r) coordinates, IPolarCoordinateSystem? system = null)
        : this(coordinates.phi, coordinates.r, system) { }

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
    public Angle Phi { get; } =
        (r >= 0 ? phi : phi + Angle.Straight).Reduce(
            system?.AngleReductionDomain ?? PolarCoordinateSystem.Default.AngleReductionDomain
        );

    /// <inheritdoc />
    public double R { get; } = Abs(r);

    /// <inheritdoc />
    public IPolarCoordinateSystem System { get; } = system ?? PolarCoordinateSystem.Default;

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

    public PolarPoint AddVector(IPolarVector v)
    {
        if (System.Equals(v.System))
        {
            var x = R * Cos(Phi) + v.R * Cos(v.Phi);
            var y = R * Sin(Phi) + v.R * Sin(v.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi =
                y > 0
                    ? Acos(x / r)
                    : y < 0
                        ? PI + Acos(-x / r)
                        : x >= 0
                            ? 0
                            : PI;

            return new PolarPoint(phi, r, System);
        }

        throw new DifferentCoordinateSystemException(this, v);
    }

    IPolarPoint IPointOperations<IPolarPoint, IPolarVector>.AddVector(IPolarVector other) =>
        AddVector(other);

    PolarPoint IPointOperations<PolarPoint, PolarVector>.AddVector(PolarVector other) =>
        AddVector(other);

    public PolarVector VectorTo(IPolarPoint p)
    {
        if (System.Equals(p.System))
        {
            var x = R * Cos(Phi) - p.R * Cos(p.Phi);
            var y = R * Sin(Phi) - p.R * Sin(p.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi =
                y > 0
                    ? Acos(x / r)
                    : y < 0
                        ? PI + Acos(-x / r)
                        : x >= 0
                            ? 0
                            : PI;

            return new PolarVector(phi, r, System);
        }

        throw new DifferentCoordinateSystemException(this, p);
    }

    IPolarVector IPointOperations<IPolarPoint, IPolarVector>.VectorTo(IPolarPoint other) =>
        VectorTo(other);

    PolarVector IPointOperations<PolarPoint, PolarVector>.VectorTo(PolarPoint other) =>
        VectorTo(other);

    public double DistanceTo(IPolarPoint other)
    {
        if (System.Equals(other.System))
            return CosLaw.C(R, other.R, Abs(other.Phi - Phi));
        return Absolute.DistanceTo(other.Absolute);
    }

    double IPointOperations<PolarPoint, PolarVector>.DistanceTo(PolarPoint other) =>
        DistanceTo(other);

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
    public PolarPoint Centroid(IPolarPoint other)
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

    IPolarPoint IPointOperations<IPolarPoint, IPolarVector>.Centroid(IPolarPoint other) =>
        Centroid(other);

    PolarPoint IPointOperations<PolarPoint, PolarVector>.Centroid(PolarPoint other) =>
        Centroid(other);

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
    public static PolarPoint operator +(PolarPoint p, PolarVector v) => p.AddVector(v);

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator -(PolarPoint p1, PolarPoint p2) => p1.VectorTo(p2);

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
