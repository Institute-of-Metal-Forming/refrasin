using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;

namespace RefraSin.ParticleModel;

public interface INodeGeometry : INodeNeighbors
{
    /// <summary>
    /// Absolute coordinates.
    /// </summary>
    public AbsolutePoint AbsoluteCoordinates => Coordinates.Absolute;

    /// <summary>
    /// Length of the surface lines to neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> SurfaceDistance => this.SurfaceDistance();

    /// <summary>
    /// Angle between radial vector and surface line.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceRadiusAngle => this.SurfaceRadiusAngle();

    /// <summary>
    /// Angle distance to neighbor nodes.
    /// </summary>
    public ToUpperToLower<Angle> AngleDistance => this.AngleDistance();

    /// <summary>
    /// Volume of the adjacent elements.
    /// </summary>
    public ToUpperToLower<double> Volume => this.Volume();

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceNormalAngle => this.SurfaceNormalAngle();

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceTangentAngle => this.SurfaceTangentAngle();
}