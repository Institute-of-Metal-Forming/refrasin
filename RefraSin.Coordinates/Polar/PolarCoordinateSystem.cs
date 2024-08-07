namespace RefraSin.Coordinates.Polar;

/// <summary>
///     Stellt ein polares Koordinatensystem dar.
/// </summary>
public class PolarCoordinateSystem : CoordinateSystem, IPolarCoordinateSystem
{
    /// <summary>
    ///     Erzeugt ein polares System mit Urspurng in Absolut(0,0), ohne Rotation, ohne Skalierung.
    /// </summary>
    public PolarCoordinateSystem() { }

    /// <summary>
    ///     Erzeugt ein polares System mit den angegebenen Parametern.
    /// </summary>
    /// <param name="origin">Ursprung, wenn null, dann wird Absolut(0,0) verwendet</param>
    /// <param name="rotationAngle">Drehwinkel</param>
    /// <param name="rScale">Skalierung entlang der R-Achse.</param>
    /// <param name="angleReductionDomain">Domain to reduce the angle coordinate to</param>
    public PolarCoordinateSystem(
        IPoint? origin = null,
        double rotationAngle = 0,
        double rScale = 1,
        Angle.ReductionDomain angleReductionDomain = Angle.ReductionDomain.None
    )
        : base(origin, rotationAngle)
    {
        RScale = rScale;
        AngleReductionDomain = angleReductionDomain;
    }

    /// <summary>
    ///     Skalierung entlang der R-Achse.
    /// </summary>
    public double RScale { get; } = 1;

    /// <summary>
    /// The angle domain where all angles of coordinates in this system are reduced to.
    /// </summary>
    public Angle.ReductionDomain AngleReductionDomain { get; } = Angle.ReductionDomain.None;

    /// <summary>
    ///     Standard-Polares System. Ursprung in (0,0), keine Drehung, keine Skalierung.
    /// </summary>
    public static PolarCoordinateSystem Default { get; } = new();

    /// <inheritdoc />
    public override string ToString() => $"{nameof(PolarCoordinateSystem)} {Label}".Trim();
}
