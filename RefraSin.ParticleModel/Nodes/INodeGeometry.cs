using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Interface of geometry data on a node.
/// </summary>
public interface INodeGeometry : INode
{
    /// <summary>
    /// Absolute coordinates.
    /// </summary>
    public AbsolutePoint AbsoluteCoordinates => Coordinates.Absolute;

    /// <summary>
    /// Length of the surface lines to neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> SurfaceDistance { get; }

    /// <summary>
    /// Angle between radial vector and surface line.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceRadiusAngle { get; }

    /// <summary>
    /// Angle distance to neighbor nodes.
    /// </summary>
    public ToUpperToLower<Angle> AngleDistance { get; }

    /// <summary>
    /// Volume of the adjacent elements.
    /// </summary>
    public ToUpperToLower<double> Volume { get; }

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceNormalAngle { get; }

    /// <summary>
    /// Angle between surface line and surface normal resp. tangent.
    /// </summary>
    public ToUpperToLower<Angle> SurfaceTangentAngle { get; }
}