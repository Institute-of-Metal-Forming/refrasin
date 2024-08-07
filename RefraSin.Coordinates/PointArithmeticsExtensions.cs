namespace RefraSin.Coordinates;

public static class PointArithmeticsExtensions
{
    public static TPoint Centroid<TPoint, TVector>(this IEnumerable<TPoint> points)
        where TPoint : IPoint, IPointArithmetics<TPoint, TVector>
        where TVector : IVector, IVectorArithmetics<TPoint, TVector>
    {
        var pointsList = points as IReadOnlyList<TPoint> ?? points.ToArray();

        return pointsList
            .Skip(1)
            .Aggregate(pointsList[0], (accumulate, point) => accumulate.Centroid(point));
    }
}
