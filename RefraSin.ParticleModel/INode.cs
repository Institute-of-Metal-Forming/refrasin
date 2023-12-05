using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

public interface INode
{
    /// <summary>
    /// Unique ID of the node.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// ID of the parent particle.
    /// </summary>
    Guid ParticleId { get; }

    /// <summary>
    /// Polar coordinates in the particle's coordinate system.
    /// </summary>
    public PolarPoint Coordinates { get; }
}