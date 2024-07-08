using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using static System.Math;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel;

public static class NodeGeometryExtensions
{
    public static ToUpperToLower<double> SurfaceDistance(this INodeGeometry self) => new(
        CosLaw.C(self.Upper.Coordinates.R, self.Coordinates.R,
            self.AngleDistance
                .ToUpper),
        CosLaw.C(self.Lower.Coordinates.R, self.Coordinates.R, self.AngleDistance.ToLower)
    );

    public static ToUpperToLower<Angle> AngleDistance(this INodeGeometry self) => new(
        self.Coordinates.AngleTo(self.Upper.Coordinates),
        self.Coordinates.AngleTo(self.Lower.Coordinates)
    );

    public static ToUpperToLower<Angle> SurfaceRadiusAngle(this INodeGeometry self) => new(
        CosLaw.Gamma(self.SurfaceDistance.ToUpper, self.Coordinates.R, self.Upper.Coordinates.R),
        CosLaw.Gamma(self.SurfaceDistance.ToLower, self.Coordinates.R, self.Lower.Coordinates.R)
    );

    public static ToUpperToLower<double> Volume(this INodeGeometry self) => new(
        0.5 * self.Coordinates.R * self.Upper.Coordinates.R * Sin(self.AngleDistance.ToUpper),
        0.5 * self.Coordinates.R * self.Lower.Coordinates.R * Sin(self.AngleDistance.ToLower)
    );

    public static ToUpperToLower<Angle> SurfaceNormalAngle(this INodeGeometry self)
    {
        if (self.Type == NodeType.Neck) // neck normal is meant normal to existing grain boundary
        {
            return self.Upper.Type == NodeType.GrainBoundary
                ? new ToUpperToLower<Angle>(HalfOfPi, ThreeHalfsOfPi - self.SurfaceRadiusAngle.ToUpper - self.SurfaceRadiusAngle.ToLower)
                : new ToUpperToLower<Angle>(ThreeHalfsOfPi - self.SurfaceRadiusAngle.ToUpper - self.SurfaceRadiusAngle.ToLower, HalfOfPi);
        }

        var angle = PI - 0.5 * (self.SurfaceRadiusAngle.ToUpper + self.SurfaceRadiusAngle.ToLower);
        return new ToUpperToLower<Angle>(angle, angle);
    }

    public static ToUpperToLower<Angle> SurfaceTangentAngle(this INodeGeometry self)
    {
        if (self.Type is NodeType.Neck) // neck tangent is meant tangential to existing grain boundary
        {
            return self.Upper.Type == NodeType.GrainBoundary
                ? new ToUpperToLower<Angle>(0, Pi - self.SurfaceRadiusAngle.ToUpper - self.SurfaceRadiusAngle.ToLower)
                : new ToUpperToLower<Angle>(Pi - self.SurfaceRadiusAngle.ToUpper - self.SurfaceRadiusAngle.ToLower, 0);
        }

        var angle = PI / 2 - 0.5 * (self.SurfaceRadiusAngle.ToUpper + self.SurfaceRadiusAngle.ToLower);
        return new ToUpperToLower<Angle>(angle, angle);
    }
}