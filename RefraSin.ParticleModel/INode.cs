using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Interface representing a node of a particle surface.
/// </summary>
public interface INode : INodeSpec
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
    public NormalTangentialAngle SurfaceAngle { get; }

    /// <summary>
    ///  Surface resp. interface energy of the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower SurfaceEnergy { get; }

    /// <summary>
    /// Diffusion coefficient at the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower SurfaceDiffusionCoefficient { get; }

    /// <summary>
    /// Gradient of Gibbs energy for node shifting in normal and tangential direction.
    /// </summary>
    public NormalTangential GibbsEnergyGradient { get; }

    /// <summary>
    /// Gradient of volume for node shifting in normal and tangential direction.
    /// </summary>
    public NormalTangential VolumeGradient { get; }
}