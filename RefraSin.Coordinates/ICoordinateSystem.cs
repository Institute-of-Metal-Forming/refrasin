namespace RefraSin.Coordinates;

/// <summary>
///     Interface for coordinate systems.
/// </summary>
public interface ICoordinateSystem
{
    /// <summary>
    ///     Rotation angle of this system's axes.
    /// </summary>
    Angle RotationAngle { get; }

    /// <summary>
    ///     Origin of this system's axes.
    /// </summary>
    IPoint Origin { get; }
}