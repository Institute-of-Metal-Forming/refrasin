namespace RefraSin.Coordinates;

public interface IVectorOperations<TVector>
    where TVector : IVector
{
    /// <summary>
    ///     Vectorial addition. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public TVector Add(TVector v);

    /// <summary>
    ///    Return the reverse of this vector.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public TVector Reverse();

    /// <summary>
    ///     Scalar multiplication. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public double ScalarProduct(TVector v);
}
