using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static RefraSin.Coordinates.Constants;
using static System.Math;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Interface of geometry data on a node.
/// </summary>
public interface INodeGeometry : INodeNeighbors
{
    /// <summary>
    /// Absolute coordinates.
    /// </summary>
    public AbsolutePoint AbsoluteCoordinates => Coordinates.Absolute;

    /// <summary>
    /// Length of the surface lines to neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> SurfaceDistance => new(
        CosLaw.C(Upper.Coordinates.R, Coordinates.R,
            AngleDistance
                .ToUpper),
        CosLaw.C(Lower.Coordinates.R, Coordinates.R, AngleDistance.ToLower)
    );

    /// <summary>
    /// Angle between radial vector and surface line.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceRadiusAngle => new(
        CosLaw.Gamma(SurfaceDistance.ToUpper, Coordinates.R, Upper.Coordinates.R),
        CosLaw.Gamma(SurfaceDistance.ToLower, Coordinates.R, Lower.Coordinates.R)
    );

    /// <summary>
    /// Angle distance to neighbor nodes.
    /// </summary>
    public ToUpperToLower<Angle> AngleDistance => new(
        Coordinates.AngleTo(Upper.Coordinates),
        Coordinates.AngleTo(Lower.Coordinates)
    );

    /// <summary>
    /// Volume of the adjacent elements.
    /// </summary>
    public ToUpperToLower<double> Volume => new(
        0.5 * Coordinates.R * Upper.Coordinates.R * Sin(AngleDistance.ToUpper),
        0.5 * Coordinates.R * Lower.Coordinates.R * Sin(AngleDistance.ToLower)
    );

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceNormalAngle
    {
        get
        {
            if (Type == NodeType.Neck) // neck normal is meant normal to existing grain boundary
            {
                return Upper.Type == NodeType.GrainBoundary
                    ? new ToUpperToLower<Angle>(HalfOfPi, ThreeHalfsOfPi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower)
                    : new ToUpperToLower<Angle>(ThreeHalfsOfPi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower, HalfOfPi);
            }

            var angle = PI - 0.5 * (SurfaceRadiusAngle.ToUpper + SurfaceRadiusAngle.ToLower);
            return new ToUpperToLower<Angle>(angle, angle);
        }
    }

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceTangentAngle
    {
        get
        {
            if (Type is NodeType.Neck) // neck tangent is meant tangential to existing grain boundary
            {
                return Upper.Type == NodeType.GrainBoundary
                    ? new ToUpperToLower<Angle>(0, Pi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower)
                    : new ToUpperToLower<Angle>(Pi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower, 0);
            }

            var angle = Pi / 2 - 0.5 * (SurfaceRadiusAngle.ToUpper + SurfaceRadiusAngle.ToLower);
            return new ToUpperToLower<Angle>(angle, angle);
        }
    }
}