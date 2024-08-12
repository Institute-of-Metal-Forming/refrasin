using RefraSin.Coordinates.Absolute;

namespace RefraSin.Coordinates;

/// <summary>
///     Schnittstelle für Punkte.
/// </summary>
public interface IPoint : ICoordinates, IPointOperations<IPoint, IVector>
{
    /// <summary>
    ///     Gets the absolute coordinate representation of this point.
    /// </summary>
    public AbsolutePoint Absolute { get; }

    /// <inheritdoc />
    IPoint IPointOperations<IPoint, IVector>.Centroid(IPoint other) =>
        Absolute.Centroid(other.Absolute);

    /// <inheritdoc />
    IPoint IPointOperations<IPoint, IVector>.AddVector(IVector v) => Absolute.AddVector(v.Absolute);

    /// <inheritdoc />
    IVector IPointOperations<IPoint, IVector>.VectorTo(IPoint p) => Absolute.VectorTo(p.Absolute);

    /// <inheritdoc />
    double IPointOperations<IPoint, IVector>.DistanceTo(IPoint p) =>
        Absolute.DistanceTo(p.Absolute);
}
