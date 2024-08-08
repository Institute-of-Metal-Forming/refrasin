using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Polar;

public readonly struct PolarVector(Angle phi, double r, IPolarCoordinateSystem? system = null)
    : IPolarVector,
        IVectorArithmetics<PolarVector>
{
    public PolarVector(IPolarCoordinateSystem? system = null)
        : this(0, 0, system) { }

    public PolarVector((Angle phi, double r) coordinates, IPolarCoordinateSystem? system = null)
        : this(coordinates.phi, coordinates.r, system) { }

    public PolarVector(IVector other, IPolarCoordinateSystem? system = null)
        : this(system)
    {
        var absoluteCoordinates = other.Absolute;
        var x = absoluteCoordinates.X;
        var y = absoluteCoordinates.Y;
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
    public bool IsClose(IPolarVector other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return R.IsClose(other.R, precision) && Phi.IsClose(other.Phi, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <inheritdoc />
    public AbsoluteVector Absolute =>
        new(
            R * System.RScale * Cos(Phi + System.RotationAngle),
            R * System.RScale * Sin(Phi + System.RotationAngle)
        );

    /// <inheritdoc />
    public double Norm => R;

    public PolarVector Add(IPolarVector v)
    {
        if (System.Equals(v.System))
        {
            var x = R * Cos(Phi) + v.R * Cos(v.Phi);
            var y = R * Sin(Phi) + v.R * Sin(v.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi = double.Atan2(y, x);
            return new PolarVector(phi, r, System);
        }

        throw new DifferentCoordinateSystemException(this, v);
    }

    IPolarVector IVectorOperations<IPolarVector>.Add(IPolarVector v) => Add(v);

    PolarVector IVectorOperations<PolarVector>.Add(PolarVector v) => Add(v);

    /// <inheritdoc />
    public PolarVector Reverse() => new(Phi + Angle.Straight, R);

    IPolarVector IVectorOperations<IPolarVector>.Reverse() => Reverse();

    public double ScalarProduct(IPolarVector v)
    {
        if (System == v.System)
            return R * v.R * Cos(Abs(Phi - v.Phi));
        throw new DifferentCoordinateSystemException(this, v);
    }

    double IVectorOperations<IPolarVector>.ScalarProduct(IPolarVector v) => ScalarProduct(v);

    double IVectorOperations<PolarVector>.ScalarProduct(PolarVector v) => ScalarProduct(v);

    /// <inheritdoc />
    public PolarVector ScaleBy(double scale) => new(Phi, R * scale);

    /// <inheritdoc />
    public PolarVector RotateBy(double rotation) => new(Phi + rotation, R);

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="PolarCoordinates.ToString(string, string, string, IFormatProvider)" />
    /// </remarks>
    public static PolarVector Parse(string s)
    {
        var (value1, value2) = s.ParseCoordinateString(nameof(PolarVector));
        return new PolarVector(Angle.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Vectorial addition. See <see cref="Add" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator +(PolarVector v1, PolarVector v2) => v1.Add(v2);

    /// <summary>
    ///     Negotiation (rotate by Pi).
    /// </summary>
    public static PolarVector operator -(PolarVector v) => v.Reverse();

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static PolarVector operator *(double d, PolarVector v) => v.ScaleBy(d);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static PolarVector operator *(PolarVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product. See <see cref="ScalarProduct" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static double operator *(PolarVector v1, PolarVector v2) => v1.ScalarProduct(v2);

    /// <inheritdoc />
    public static PolarVector operator /(PolarVector left, double right) =>
        new(left.Phi, left.R / right);

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatPolarCoordinates(format, formatProvider);

    /// <inheritdoc />
    public double[] ToArray() => [Phi, R];

    /// <inheritdoc />
    public static PolarVector operator -(PolarVector left, PolarVector right) => left + -right;
}
