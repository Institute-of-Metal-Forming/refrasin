namespace RefraSin.Coordinates.Polar;

public interface IPolarCoordinateSystem : ICoordinateSystem
{
    /// <summary>
    ///     Skalierung entlang der R-Achse.
    /// </summary>
    double RScale { get; }

    /// <summary>
    /// The angle domain where all angles of coordinates in this system are reduced to.
    /// </summary>
    Angle.ReductionDomain AngleReductionDomain { get; }
    
}