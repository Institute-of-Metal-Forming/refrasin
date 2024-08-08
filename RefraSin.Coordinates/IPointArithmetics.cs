using System.Numerics;

namespace RefraSin.Coordinates;

public interface IPointArithmetics<TPoint, TVector>
    : IPointOperations<TPoint, TVector>,
        IAdditionOperators<TPoint, TVector, TPoint>,
        ISubtractionOperators<TPoint, TPoint, TVector>,
        ICoordinateTransformations<TPoint>
    where TPoint : IPoint, IPointArithmetics<TPoint, TVector>
    where TVector : IVector;
