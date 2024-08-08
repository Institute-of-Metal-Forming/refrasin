using System.Numerics;

namespace RefraSin.Coordinates;

public interface IVectorArithmetics<TVector>
    : IVectorOperations<TVector>,
        IAdditionOperators<TVector, TVector, TVector>,
        ISubtractionOperators<TVector, TVector, TVector>,
        IMultiplyOperators<TVector, double, TVector>,
        IDivisionOperators<TVector, double, TVector>,
        IUnaryNegationOperators<TVector, TVector>,
        ICoordinateTransformations<TVector>
    where TVector : IVector, IVectorArithmetics<TVector>;
