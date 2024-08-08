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

    /// <inheritdoc />
    public PolarVector Add(PolarVector v) => this + v;

    /// <inheritdoc />
    public PolarVector Subtract(PolarVector v) => this - v;

    /// <inheritdoc />
    public double ScalarProduct(PolarVector v) => this * v;

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
    public static PolarVector operator +(PolarVector v1, PolarVector v2)
    {
        if (v1.System.Equals(v2.System))
        {
            var x = v1.R * Cos(v1.Phi) + v2.R * Cos(v2.Phi);
            var y = v1.R * Sin(v1.Phi) + v2.R * Sin(v2.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi = double.Atan2(y, x);
            return new PolarVector(phi, r, v1.System);
        }

        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <summary>
    ///     Vectorial subtraction. See <see cref="Subtract" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator -(PolarVector v1, PolarVector v2)
    {
        if (v1.System.Equals(v2.System))
        {
            var x = v1.R * Cos(v1.Phi) - v2.R * Cos(v2.Phi);
            var y = v1.R * Sin(v1.Phi) - v2.R * Sin(v2.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi =
                y > 0
                    ? Acos(x / r)
                    : y < 0
                        ? PI + Acos(-x / r)
                        : x >= 0
                            ? 0
                            : PI;

            return new PolarVector(phi, r, v1.System);
        }

        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <summary>
    ///     Negotiation (rotate by Pi).
    /// </summary>
    public static PolarVector operator -(PolarVector v) => new(v.Phi + PI, v.R);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static PolarVector operator *(double d, PolarVector v) => new(v.Phi, d * v.R, v.System);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static PolarVector operator *(PolarVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product. See <see cref="ScalarProduct" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static double operator *(PolarVector v1, PolarVector v2)
    {
        if (v1.System == v2.System)
            return v1.R * v2.R * Cos(Abs(v1.Phi - v2.Phi));
        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <inheritdoc />
    public static PolarVector operator /(PolarVector left, double right) =>
        new(left.Phi, left.R / right);

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatPolarCoordinates(format, formatProvider);

    /// <inheritdoc />
    public double[] ToArray() => [Phi, R];
}
