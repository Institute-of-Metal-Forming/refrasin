namespace RefraSin.Coordinates.Polar;

public interface IPolarCoordinates : ICoordinates
{
    /// <summary>
    ///     Gets or sets the angle coordinate.
    /// </summary>
    Angle Phi { get; }

    /// <summary>
    ///     Gets or sets the radius coordinate.
    ///     <remarks>
    ///         Is always positive. Setting of negative values will cause to R > 0 and Phi += Pi.
    ///     </remarks>
    /// </summary>
    double R { get; }

    /// <summary>
    ///     Coordinate system where these coordinates are defined in.
    /// </summary>
    IPolarCoordinateSystem System { get; }

    /// <summary>
    ///     Compute the angle distance between two sets of coordinates.
    /// </summary>
    /// <param name="other">other</param>
    /// <param name="allowNegative">whether to allow negative return values indicating direction</param>
    /// <returns>angle distance in interval [0; Pi], or [-Pi; Pi] if <paramref name="allowNegative" /> == true</returns>
    Angle AngleTo(IPolarCoordinates other, bool allowNegative = false);
}