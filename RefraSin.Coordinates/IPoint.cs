using RefraSin.Coordinates.Absolute;

namespace RefraSin.Coordinates;

/// <summary>
///     Schnittstelle für Punkte.
/// </summary>
public interface IPoint : ICoordinates
{
    /// <summary>
    ///     Gets the absolute coordinate representation of this point.
    /// </summary>
    public AbsolutePoint Absolute { get; }
}
