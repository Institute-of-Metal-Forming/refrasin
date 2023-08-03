using RefraSin.Coordinates.Absolute;

namespace RefraSin.Coordinates;

/// <summary>
///     Schnittstelle für Punkte.
/// </summary>
public interface IPoint : ICoordinates, ICloneable<IPoint>
{
    /// <summary>
    ///     Gets the absolute coordinate representation of this point.
    /// </summary>
    public AbsolutePoint Absolute { get; }

    /// <summary>
    ///     Addition of a vector to this point. Only defined for points and vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public IPoint AddVector(IVector v);

    /// <summary>
    ///     Gets the vector between two points. Only defined for points of the same coordinate system.
    /// </summary>
    /// <param name="p">other point</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public IVector VectorTo(IPoint p);

    /// <summary>
    ///     Gets euclidean distance between two points. Only defined for points of the same coordinate system.
    /// </summary>
    /// <param name="p">other point</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public double DistanceTo(IPoint p);
}