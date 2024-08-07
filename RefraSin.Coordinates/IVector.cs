using RefraSin.Coordinates.Absolute;

namespace RefraSin.Coordinates;

/// <summary>
///     Schnittstelle für Vektoren.
/// </summary>
public interface IVector : ICoordinates
{
    /// <summary>
    ///     Gets the absolute coordinate representation of this vector.
    /// </summary>
    public AbsoluteVector Absolute { get; }

    /// <summary>
    ///     Gets the euclidean norm of this vector.
    /// </summary>
    public double Norm { get; }
}
