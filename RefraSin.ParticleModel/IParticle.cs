using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

public interface IParticle : IVertex
{
    /// <summary>
    /// Absolute coordinates of the particle's center.
    /// </summary>
    public AbsolutePoint CenterCoordinates { get; }

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
    public IReadOnlyParticleSurface<INodeGeometry> Nodes { get; }
}