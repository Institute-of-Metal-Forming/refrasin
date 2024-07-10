namespace RefraSin.Coordinates.Cartesian;

public interface ICartesianCoordinateSystem : ICoordinateSystem
{
    /// <summary>
    ///     Skalierung entlang der X-Achse.
    /// </summary>
    double XScale { get; }
    
    /// <summary>
    ///     Skalierung entlang der X-Achse.
    /// </summary>
    double YScale { get; }
}