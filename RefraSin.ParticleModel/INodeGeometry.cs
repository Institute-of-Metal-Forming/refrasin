using RefraSin.Coordinates.Absolute;

namespace RefraSin.ParticleModel;

public interface INodeGeometry
{
    /// <summary>
    /// Absolute coordinates.
    /// </summary>
    public AbsolutePoint AbsoluteCoordinates { get; }

    /// <summary>
    /// Length of the surface lines to neighbor nodes.
    /// </summary>
    public ToUpperToLower SurfaceDistance { get; }

    /// <summary>
    /// Angle between radial vector and surface line.
    /// </summary>
    public ToUpperToLowerAngle SurfaceRadiusAngle { get; }

    /// <summary>
    /// Angle distance to neighbor nodes.
    /// </summary>
    public ToUpperToLowerAngle AngleDistance { get; }

    /// <summary>
    /// Volume of the adjacent elements.
    /// </summary>
    public ToUpperToLower Volume { get; }

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public NormalTangentialAngle SurfaceVectorAngle { get; }
}