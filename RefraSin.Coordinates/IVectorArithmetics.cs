using System.Numerics;

namespace RefraSin.Coordinates;

public interface IVectorArithmetics<TVector>
    : IVectorOperations<TVector>,
        IAdditionOperators<TVector, TVector, TVector>,
        IMultiplyOperators<TVector, double, TVector>,
        IDivisionOperators<TVector, double, TVector>,
        IUnaryNegationOperators<TVector, TVector>,
        ICoordinateTransformations<TVector>
    where TVector : IVector, IVectorArithmetics<TVector>;

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
    ///     Vectorial subtraction. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public TVector Subtract(TVector v);

    /// <summary>
    ///     Scalar multiplication. Only defined for vectors of the same coordinate system.
    /// </summary>
    /// <param name="v">other vector</param>
    /// <returns></returns>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public double ScalarProduct(TVector v);
}
