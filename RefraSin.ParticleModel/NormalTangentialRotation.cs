using RefraSin.Coordinates;
using static System.Math;

namespace RefraSin.ParticleModel;

/// <summary>
/// Structure encapsulating quantities with two components, one in normal direction, one in tangential.
/// </summary>
public readonly struct NormalTangentialRotation(double normal, double tangential, double rotation)
{
    /// <summary>
    /// Normal component.
    /// </summary>
    public readonly double Normal = normal;

    /// <summary>
    /// Tangential component.
    /// </summary>
    public readonly double Tangential = tangential;

    /// <summary>
    /// Rotation component.
    /// </summary>
    public readonly double Rotation = rotation;

    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public double[] ToArray() => new[] { Normal, Tangential, Rotation };

    public static implicit operator NormalTangentialRotation((double Normal, double Tangential, double Rotation) t) =>
        new(t.Normal, t.Tangential, t.Rotation);
}

/// <summary>
/// Structure encapsulating angle quantities with two components, one in normal direction, one in tangential.
/// </summary>
public readonly struct NormalTangentialRotationAngle(Angle normal, Angle tangential, Angle rotation)
{
    /// <summary>
    /// Normal component.
    /// </summary>
    public readonly Angle Normal = normal;

    /// <summary>
    /// Tangential component.
    /// </summary>
    public readonly Angle Tangential = tangential;

    /// <summary>
    /// Rotation component.
    /// </summary>
    public readonly Angle Rotation = rotation;

    public static implicit operator NormalTangentialRotation(NormalTangentialRotationAngle other) =>
        new(other.Normal, other.Tangential, other.Rotation);

    public static implicit operator NormalTangentialRotationAngle(NormalTangentialRotation other) =>
        new(other.Normal, other.Tangential, other.Rotation);

    public static implicit operator NormalTangentialRotationAngle((double Normal, double Tangential, double Rotation) t) =>
        new(t.Normal, t.Tangential, t.Rotation);

    public static implicit operator NormalTangentialRotationAngle((Angle Normal, Angle Tangential, Angle Rotation) t) =>
        new(t.Normal, t.Tangential, t.Rotation);

    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public Angle[] ToArray() => new[] { Normal, Tangential, Rotation };

    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public double[] ToDoubleArray() => new double[] { Normal, Tangential, Rotation };
}