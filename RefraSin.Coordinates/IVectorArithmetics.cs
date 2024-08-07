using System.Numerics;

namespace RefraSin.Coordinates;

public interface IVectorArithmetics<TPoint, TVector>
    : IAdditionOperators<TVector, TVector, TVector>,
        IMultiplyOperators<TVector, double, TVector>,
        IDivisionOperators<TVector, double, TVector>,
        IMultiplyOperators<TVector, TVector, double>
    where TPoint : IPoint
    where TVector : IVector, IVectorArithmetics<TPoint, TVector>;
