using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

/// <summary>
/// Interface representing a powder particle.
/// </summary>
public interface IParticle : IParticleSpec
{
    /// <summary>
    /// Polar coordinates of the particle's center in means of the parent's coordinate system.
    /// </summary>
    public PolarPoint CenterCoordinates { get; }

    /// <summary>
    /// List of all surface nodes.
    /// </summary>
    public IReadOnlyList<INode> Nodes { get; }

    /// <summary>
    /// List of all necks to neighbor particles.
    /// </summary>
    public IReadOnlyList<INeck> Necks { get; }
}