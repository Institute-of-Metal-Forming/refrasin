namespace RefraSin.Coordinates;

/// <summary>
///     Interface for coordinates.
/// </summary>
public interface ICoordinates
{
    /// <summary>
    ///     Rotates the coordinates with the systems origin as center.
    /// </summary>
    /// <param name="angle">rotation angle</param>
    public void RotateBy(Angle angle);

    /// <summary>
    /// Returns the coordinates as 2-element array.
    /// </summary>
    /// <returns></returns>
    public double[] ToArray();
}