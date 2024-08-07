using RefraSin.Coordinates.Absolute;

namespace RefraSin.Coordinates;

/// <summary>
///     Schnittstelle für Vektoren.
/// </summary>
public interface IVector : ICoordinates, ICloneable<IVector>
{
    /// <summary>
    ///     Gets the absolute coordinate representation of this vector.
    /// </summary>
    public AbsoluteVector Absolute { get; }

    /// <summary>
    ///     Gets the euclidean norm of this vector.
    /// </summary>
    public double Norm { get; }

    /// <summary>
    ///     Vectorial addition. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public IVector Add(IVector v);

    /// <summary>
    ///     Vectorial subtraction. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public IVector Subtract(IVector v);

    /// <summary>
    ///     Scalar multiplication. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public double ScalarProduct(IVector v);

    /// <summary>
    ///     Scale the vector.
    /// </summary>
    /// <param name="scale">scale factor</param>
    public void ScaleBy(double scale);
}
