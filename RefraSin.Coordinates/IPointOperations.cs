namespace RefraSin.Coordinates;

public interface IPointOperations<TPoint, TVector>
    where TPoint : IPoint
    where TVector : IVector
{
    public TPoint Centroid(TPoint other);

    /// <summary>
    ///     Addition of a vector to this point. Only defined for points and vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public TPoint AddVector(TVector v);

    /// <summary>
    ///     Gets the vector between two points. Only defined for points of the same coordinate system.
    /// </summary>
    /// <param name="p">other point</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public TVector VectorTo(TPoint p);

    /// <summary>
    ///     Gets euclidean distance between two points. Only defined for points of the same coordinate system.
    /// </summary>
    /// <param name="p">other point</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public double DistanceTo(TPoint p);
}