using System.Numerics;

namespace RefraSin.Coordinates;

public interface IPointArithmetics<TPoint, TVector>
    : IAdditionOperators<TPoint, TVector, TPoint>,
        ISubtractionOperators<TPoint, TPoint, TVector>
    where TPoint : IPoint, IPointArithmetics<TPoint, TVector>
    where TVector : IVector
{
    public TPoint Centroid(TPoint other);

    public TPoint Centroid(IPoint other);
}
