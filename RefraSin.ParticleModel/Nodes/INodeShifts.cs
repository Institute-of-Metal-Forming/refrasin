namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Interface of shift data on a node.
/// </summary>
public interface INodeShifts : INode
{
    /// <summary>
    /// Shift distance of the node in normal and tangential directions.
    /// </summary>
    NormalTangential<double> Shift { get; }
}