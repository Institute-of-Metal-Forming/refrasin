using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;

namespace RefraSin.ParticleModel;

public interface IParticleSpec
{
    /// <summary>
    /// Unique ID.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Absolute coordinates of the particle's center.
    /// </summary>
    public AbsolutePoint AbsoluteCenterCoordinates { get; }

    /// <summary>
    /// Rotation angle of the particle's coordinate system around its center.
    /// </summary>
    Angle RotationAngle { get; }

    /// <summary>
    /// ID of the material.
    /// </summary>
    Guid MaterialId { get; }

    /// <summary>
    /// List of node specs.
    /// </summary>
    public IReadOnlyList<INodeSpec> NodeSpecs { get; }
}