using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

public interface IParticleSpec : IVertex
{
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

    /// <summary>
    /// Returns the node on index i, while the index is considered ringwise.
    /// </summary>
    /// <param name="i"></param>
    public INodeSpec this[int i] { get; }

    /// <summary>
    /// Returns the node with the given Id.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <exception cref="IndexOutOfRangeException">if a node with given Id is not part of this particle</exception>
    public INodeSpec this [Guid nodeId] { get; }
}