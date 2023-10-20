using RefraSin.Coordinates;
using static System.Math;

namespace RefraSin.ParticleModel;

/// <summary>
/// Structure encapsulating quantities with two components, one in normal direction, one in tangential.
/// </summary>
public struct NormalTangential
{
    public NormalTangential(double normal, double tangential)
    {
        Normal = normal;
        Tangential = tangential;
        Sum = Sqrt(Pow(Normal, 2) + Pow(Tangential, 2));
    }

    /// <summary>
    /// Normal component.
    /// </summary>
    public readonly double Normal;

    /// <summary>
    /// Tangential component.
    /// </summary>
    public readonly double Tangential;

    /// <summary>
    /// Vector sum (euclidian norm) of the components.
    /// </summary>
    /// <returns></returns>
    public readonly double Sum;

    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public double[] ToArray() => new[] { Normal, Tangential };

    public static implicit operator NormalTangential(double both) => new(both, both);
    public static implicit operator NormalTangential((double Normal, double Tangential) t) => new(t.Normal, t.Tangential);
}

/// <summary>
/// Structure encapsulating angle quantities with two components, one in normal direction, one in tangential.
/// </summary>
public struct NormalTangentialAngle
{
    public NormalTangentialAngle(Angle normal, Angle tangential)
    {
        Normal = normal;
        Tangential = tangential;
    }

    /// <summary>
    /// Normal component.
    /// </summary>
    public readonly Angle Normal;

    /// <summary>
    /// Tangential component.
    /// </summary>
    public readonly Angle Tangential;

    public static implicit operator NormalTangential(NormalTangentialAngle other) => new(other.Normal, other.Tangential);

    public static implicit operator NormalTangentialAngle(NormalTangential other) => new(other.Normal, other.Tangential);
    public static implicit operator NormalTangentialAngle(double both) => new(both, both);
    public static implicit operator NormalTangentialAngle((double Normal, double Tangential) t) => new(t.Normal, t.Tangential);
    public static implicit operator NormalTangentialAngle(Angle both) => new(both, both);
    public static implicit operator NormalTangentialAngle((Angle Normal, Angle Tangential) t) => new(t.Normal, t.Tangential);

    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public Angle[] ToArray() => new[] { Normal, Tangential };

    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public double[] ToDoubleArray() => new double[] { Normal, Tangential };
}