namespace RefraSin.Coordinates;

/// <summary>
///     Interface for coordinates.
/// </summary>
public interface ICoordinates : IFormattable
{
    /// <summary>
    /// Returns the coordinates as 2-element array.
    /// </summary>
    /// <returns></returns>
    public double[] ToArray();
}
