namespace RefraSin.Coordinates.Cartesian;

public interface ICartesianCoordinates : ICoordinates
{
    /// <summary>
    ///     Gets or sets the horizontal coordinate X.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    ///     Gets or sets the vertical coordinate Y.
    /// </summary>
    public double Y { get; set; }


    /// <summary>
    ///     Coordinate system where these coordinates are defined in.
    /// </summary>
    ICartesianCoordinateSystem System { get; }
}