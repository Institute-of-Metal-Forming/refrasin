using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel.Nodes;

public interface INode : IVertex
{
    /// <summary>
    /// ID of the parent particle.
    /// </summary>
    Guid ParticleId { get; }

    /// <summary>
    /// Polar coordinates in the particle's coordinate system.
    /// </summary>
    public IPolarPoint Coordinates { get; }

    /// <summary>
    /// Type of the node.
    /// </summary>
    public NodeType Type { get; }
}