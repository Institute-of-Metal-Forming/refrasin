using RefraSin.Coordinates.Helpers;
using static RefraSin.Coordinates.Angle.ReductionDomain;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.Coordinates;

/// <summary>
///     Represents an angle. Can be implicitly converted from and to double (always in radians).
/// </summary>
public readonly struct Angle : IFormattable, IIsClose<Angle>
{
    /// <summary>
    ///     Constructs an angle from its value given in radians.
    /// </summary>
    /// <param name="radians">Angle in radians</param>
    public Angle(double radians) : this()
    {
        Radians = radians;
    }

    /// <summary>
    ///     Constructs an angle from its value given in radians reduced to a specified domain.
    /// </summary>
    /// <param name="radians">Angle in radians</param>
    /// <param name="domain">the domain</param>
    public Angle(double radians, ReductionDomain domain) : this()
    {
        Radians = ReduceRadians(radians, domain);
    }

    /// <summary>
    ///     Angle in radians.
    /// </summary>
    public double Radians { get; }

    /// <summary>
    ///     Angle in degrees.
    /// </summary>
    public double Degrees => Radians / Pi * 180;

    /// <summary>
    ///     Angle in gradians (gon).
    /// </summary>
    public double Gradians => Radians / Pi * 200;

    /// <summary>
    ///     Reduces the angle given in radians to a specified domain.
    /// </summary>
    /// <param name="radians">the angle in radians</param>
    /// <param name="domain">the domain</param>
    /// <returns>reduced angle in radians</returns>
    /// <exception cref="NotSupportedException">if the specification of the <paramref name="domain" /> is invalid</exception>
    public static double ReduceRadians(double radians, ReductionDomain domain)
    {
        switch (domain)
        {
            case None:
                return radians;

            case AllPositive:
            {
                radians %= TwoPi;
                if (radians >= TwoPi)
                    radians -= TwoPi;
                else if (radians < 0)
                    radians += TwoPi;
                return radians;
            }

            case WithNegative:
            {
                radians %= TwoPi;
                if (radians >= Pi)
                    radians -= TwoPi;
                else if (radians < -Pi)
                    radians += TwoPi;
                return radians;
            }
            default:
                throw new NotSupportedException("Invalid domain specification.");
        }
    }

    /// <summary>
    ///     Reduces the angle given in degrees to a specified domain.
    /// </summary>
    /// <param name="degrees">the angle in degrees</param>
    /// <param name="domain">the domain</param>
    /// <returns>reduced angle in degrees</returns>
    /// <exception cref="NotSupportedException">if the specification of the <paramref name="domain" /> is invalid</exception>
    public static double ReduceDegrees(double degrees, ReductionDomain domain)
    {
        switch (domain)
        {
            case None:
                return degrees;

            case AllPositive:
            {
                degrees %= 360;
                if (degrees >= 360)
                    degrees -= 360;
                else if (degrees < 0)
                    degrees += 360;
                return degrees;
            }

            case WithNegative:
            {
                degrees %= 360;
                if (degrees >= 180)
                    degrees -= 360;
                else if (degrees < -180)
                    degrees += 360;
                return degrees;
            }
            default:
                throw new NotSupportedException("Invalid domain specification.");
        }
    }

    /// <summary>
    ///     Reduces the angle given in gradians to the domain [0, 400 gon].
    /// </summary>
    /// <param name="gradians">the angle in gradians</param>
    /// <param name="domain">the domain</param>
    /// <returns>reduced angle in gradians</returns>
    /// <exception cref="NotSupportedException">if the specification of the <paramref name="domain" /> is invalid</exception>
    public static double ReduceGradians(double gradians, ReductionDomain domain)
    {
        switch (domain)
        {
            case None:
                return gradians;

            case AllPositive:
            {
                gradians %= 400;
                if (gradians >= 400)
                    gradians -= 400;
                else if (gradians < 0)
                    gradians += 400;
                return gradians;
            }

            case WithNegative:
            {
                gradians %= 400;
                if (gradians >= 200)
                    gradians -= 400;
                else if (gradians < -200)
                    gradians += 400;
                return gradians;
            }
            default:
                throw new NotSupportedException("Invalid domain specification.");
        }
    }

    /// <summary>
    ///     Returns this angle reduced on [0, 2π] resp. [0, 360°] resp. [0, 400 gon].
    /// </summary>
    /// <returns></returns>
    public Angle Reduce() => FromRadians(ReduceRadians(Radians, AllPositive));

    /// <summary>
    ///     Returns this angle reduced on a specified domain.
    /// </summary>
    /// <returns></returns>
    public Angle Reduce(ReductionDomain domain) => FromRadians(ReduceRadians(Radians, domain));

    /// <summary>
    /// Determines if the value of this angle lies in a specified domain.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns>true, if the angle lies in the domain, otherwise false</returns>
    /// <exception cref="NotSupportedException">invalid domain specification</exception>
    public bool IsInDomain(ReductionDomain domain)
    {
        switch (domain)
        {
            case None:
                return true;

            case AllPositive:
            {
                if (Radians >= TwoPi || Radians < 0)
                    return false;
                return true;
            }

            case WithNegative:
            {
                if (Radians >= Pi || Radians < -Pi)
                    return false;
                return true;
            }
            default:
                throw new NotSupportedException("Invalid domain specification.");
        }
    }

    /// <summary>
    ///     Implicit conversion from Angle to double (radians).
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static implicit operator double(Angle angle) => angle.Radians;

    /// <summary>
    ///     Implicit conversion from double to Angle (radians).
    /// </summary>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static implicit operator Angle(double radians) => new(radians);

    /// <summary>
    ///     Constructs an angle from a double representing radians.
    /// </summary>
    /// <param name="radians">value in radians</param>
    /// <returns></returns>
    public static Angle FromRadians(double radians) => new(radians);

    /// <summary>
    ///     Constructs an angle from a double representing radians and reduces to a specified domain.
    /// </summary>
    /// <param name="radians">value in radians</param>
    /// <param name="domain">the domain</param>
    /// <returns></returns>
    public static Angle FromRadians(double radians, ReductionDomain domain) => FromRadians(ReduceRadians(radians, domain));

    /// <summary>
    ///     Constructs an angle from a double representing degrees.
    /// </summary>
    /// <param name="degrees">value in degrees</param>
    /// <returns></returns>
    public static Angle FromDegrees(double degrees) => new(degrees / 180 * Pi);

    /// <summary>
    ///     Constructs an angle from a double representing degrees and reduces to a specified domain.
    /// </summary>
    /// <param name="degrees">value in degrees</param>
    /// <param name="domain">the domain</param>
    /// <returns></returns>
    public static Angle FromDegrees(double degrees, ReductionDomain domain) => FromDegrees(ReduceDegrees(degrees, domain));

    /// <summary>
    ///     Constructs an angle from a double representing gradians.
    /// </summary>
    /// <param name="gradians">value in gradians</param>
    /// <returns></returns>
    public static Angle FromGradians(double gradians) => new(gradians / 200 * Pi);

    /// <summary>
    ///     Constructs an angle from a double representing gradians and reduces to a specified domain.
    /// </summary>
    /// <param name="gradians">value in gradians</param>
    /// <param name="domain">the domain</param>
    /// <returns></returns>
    public static Angle FromGradians(double gradians, ReductionDomain domain) => FromGradians(ReduceGradians(gradians, domain));

    /// <summary>
    ///     String representation of this angle.
    /// </summary>
    public override string ToString() => ToString(null, null);

    /// <summary>
    ///     String representation of this angle.
    /// </summary>
    /// <param name="formatProvider">Format provider to pass to double.ToString()</param>
    public string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

    /// <summary>
    ///     String representation of this angle.
    /// </summary>
    /// <param name="format">
    ///     Format string "angleFormat:numberFormat", see <see cref="ToString(String,String,IFormatProvider)" />.
    /// </param>
    public string ToString(string format) => ToString(format, null);

    /// <summary>
    ///     String representation of this angle.
    /// </summary>
    /// <param name="format">
    ///     Format string "angleFormat:numberFormat", see <see cref="ToString(String,String,IFormatProvider)" />.
    /// </param>
    /// <param name="formatProvider">Format provider to pass to double.ToString()</param>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            format = "R";

        var formats = format.Split(':');

        var numberFormat = formats[0];
        var angleFormat = formats.Length > 1 ? formats[1] : null;

        return ToString(numberFormat, angleFormat, formatProvider);
    }

    /// <summary>
    ///     String representation of this angle.
    /// </summary>
    /// <param name="numberFormat">format of the number, see <see cref="double.ToString(String, IFormatProvider)" /></param>
    /// <param name="angleFormat">format of angle unit</param>
    /// <param name="formatProvider">Format provider to pass to double.ToString()</param>
    /// <remarks>
    ///     valid format specifiers for <paramref name="angleFormat" /> are
    ///     <list type="table">
    ///         <listheader>
    ///             <term>format specifier</term>
    ///             <description>meaning</description>
    ///         </listheader>
    ///         <item>
    ///             <term>rad, null, G or empty</term>
    ///             <description>radians (rad)</description>
    ///         </item>
    ///         <item>
    ///             <term>deg</term>
    ///             <description>degrees (°)</description>
    ///         </item>
    ///         <item>
    ///             <term>gon</term>
    ///             <description>gradians (gon)</description>
    ///         </item>
    ///         <item>
    ///             <term>grad</term>
    ///             <description>gradians (grad)</description>
    ///         </item>
    ///         <item>
    ///             <term>rad</term>
    ///             <description>radians</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public string ToString(string? numberFormat, string? angleFormat, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(angleFormat))
            angleFormat = "G";

        ReductionDomain? domain = null;
        if (angleFormat.Contains('+'))
            domain = AllPositive;
        else if (angleFormat.Contains('-'))
            domain = WithNegative;

        if (angleFormat.Contains("rad") || angleFormat.Contains("G"))
        {
            var value = domain != null ? ReduceRadians(Radians, domain.Value) : Radians;
            return $"{value.ToString(numberFormat, formatProvider)} rad";
        }

        if (angleFormat.Contains("deg"))
        {
            var value = domain != null ? ReduceDegrees(Degrees, domain.Value) : Degrees;
            return $"{value.ToString(numberFormat, formatProvider)}°";
        }

        if (angleFormat.Contains("grad"))
        {
            var value = domain != null ? ReduceGradians(Gradians, domain.Value) : Gradians;
            return $"{value.ToString(numberFormat, formatProvider)} grad";
        }

        if (angleFormat.Contains("gon"))
        {
            var value = domain != null ? ReduceGradians(Gradians, domain.Value) : Gradians;
            return $"{value.ToString(numberFormat, formatProvider)} gon";
        }

        throw new FormatException($"Invalid format string: {angleFormat}");
    }

    /// <summary>
    ///     Parse this angle from a string. Inverts Angle.ToString(). Angle unit is automatically recognized.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Angle Parse(string s) => Parse(s, null);

    /// <summary>
    ///     Parse this angle from a string. Inverts Angle.ToString(). Angle unit is automatically recognized.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="formatProvider">Format provider to pass to double.Parse()</param>
    /// <returns></returns>
    public static Angle Parse(string s, IFormatProvider? formatProvider)
    {
        s = s.Trim();

        if (s.EndsWith("gon") || s.EndsWith("grad"))
            return FromGradians(double.Parse(s.Substring(0, s.IndexOf('g')).Trim(), formatProvider));
        if (s.EndsWith("rad"))
            return FromRadians(double.Parse(s.Substring(0, s.IndexOf('r')).Trim(), formatProvider));
        if (s.EndsWith("°"))
            return FromDegrees(double.Parse(s.Substring(0, s.IndexOf('°')).Trim(), formatProvider));

        throw new FormatException("Format not recognized.");
    }

    /// <summary>
    ///     Addition.
    /// </summary>
    public static Angle operator +(Angle a1, Angle a2) => new(a1.Radians + a2.Radians);

    /// <summary>
    ///     Addition.
    /// </summary>
    public static Angle operator +(Angle a, double d) => new(a.Radians + d);

    /// <summary>
    ///     Addition.
    /// </summary>
    public static Angle operator +(double d, Angle a) => new(a.Radians + d);

    /// <summary>
    ///     Subtraction.
    /// </summary>
    public static Angle operator -(Angle a1, Angle a2) => new(a1.Radians - a2.Radians);

    /// <summary>
    ///     Subtraction.
    /// </summary>
    public static Angle operator -(Angle a, double d) => new(a.Radians - d);

    /// <summary>
    ///     Negation.
    /// </summary>
    public static Angle operator -(Angle a) => new(-a.Radians);

    /// <summary>
    ///     Subtraction.
    /// </summary>
    public static Angle operator -(double d, Angle a) => new(d - a.Radians);

    /// <summary>
    ///     Scalar multiplication.
    /// </summary>
    public static Angle operator *(Angle a, double d) => new(a.Radians * d);

    /// <summary>
    ///     Scalar multiplication.
    /// </summary>
    public static Angle operator *(double d, Angle a) => new(a.Radians * d);

    /// <summary>
    ///     Scalar division.
    /// </summary>
    public static Angle operator /(Angle a, double d) => new(a.Radians / d);

    /// <summary>
    ///     Greater. Determines if <paramref name="a1" /> is in the half circle above <paramref name="a2" />.
    ///     For <paramref name="a1" />=<paramref name="a2" />+Pi always false.
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <returns></returns>
    public static bool operator >(Angle a1, Angle a2)
    {
        var diff = (a1 - a2).Reduce(WithNegative);
        if (diff.Radians > 0 && diff.Radians < Pi)
            return true;
        return false;
    }

    /// <summary>
    ///     Smaller. Determines if <paramref name="a1" /> is in the half circle below <paramref name="a2" />.
    ///     For <paramref name="a1" />=<paramref name="a2" />+Pi always false.
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <returns></returns>
    public static bool operator <(Angle a1, Angle a2)
    {
        var diff = (a1 - a2).Reduce(WithNegative);
        if (diff.Radians < 0 && diff.Radians > -Pi)
            return true;
        return false;
    }

    /// <summary>
    ///     Greater or equal. Determines if <paramref name="a1" /> is in the half circle above <paramref name="a2" />.
    ///     For <paramref name="a1" />=<paramref name="a2" />+Pi always false.
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <returns></returns>
    public static bool operator >=(Angle a1, Angle a2) => a1.IsClose(a2) || a1 > a2;

    /// <summary>
    ///     Smaller or equal. Determines if <paramref name="a1" /> is in the half circle below <paramref name="a2" />.
    ///     For <paramref name="a1" />=<paramref name="a2" />+Pi always false.
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <returns></returns>
    public static bool operator <=(Angle a1, Angle a2) => a1.IsClose(a2) || a1 < a2;

    /// <summary>
    ///     Compares if equal within maximum error.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public bool IsClose(Angle other, double precision = 1e-8) => (Radians - other.Radians).IsClose(0, precision);

    /// <summary>
    /// Constant of full angle (2 Pi or 360°).
    /// </summary>
    public static readonly Angle Full = new(TwoPi);

    /// <summary>
    /// Constant of full angle (2 Pi or 360°).
    /// </summary>
    public static readonly Angle Zero = new(0);

    /// <summary>
    /// Constant of half angle (Pi or 180°). Equivalent to <see cref="Straight"/>.
    /// </summary>
    public static readonly Angle Half = new(Pi);

    /// <summary>
    /// Constant of straight angle (Pi or 180°). Equivalent to <see cref="Half"/>.
    /// </summary>
    public static readonly Angle Straight = Half;

    /// <summary>
    /// Constant of half angle (0.5 Pi or 90°). Equivalent to <see cref="Right"/>.
    /// </summary>
    public static readonly Angle Quarter = new(HalfOfPi);

    /// <summary>
    /// Constant of half angle (Pi/2 or 90°). Equivalent to <see cref="Quarter"/>.
    /// </summary>
    public static readonly Angle Right = Quarter;

    /// <summary>
    /// Constant of eighth angle (Pi/4 or 45°). Equivalent to <see cref="HalfRight"/>.
    /// </summary>
    public static readonly Angle Eighth = new(QuarterOfPi);

    /// <summary>
    /// Constant of half right angle (Pi/4 or 45°). Equivalent to <see cref="Eighth"/>.
    /// </summary>
    public static readonly Angle HalfRight = Eighth;

    /// <summary>
    ///     Represents a domain of an angle to be reduced to.
    /// </summary>
    public enum ReductionDomain
    {
        /// <summary>
        /// No reduction.
        /// </summary>
        None,

        /// <summary>
        ///     Domain [0, 2π] resp. [0, 360°] resp. [0, 400 gon].
        /// </summary>
        AllPositive,

        /// <summary>
        ///     Domain [-π, π] resp. [-180°, 180°] resp. [-200 gon, 200 gon].
        /// </summary>
        WithNegative
    }
}