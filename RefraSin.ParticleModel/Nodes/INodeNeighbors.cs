namespace RefraSin.ParticleModel.Nodes;

public interface INodeNeighbors
{
    /// <summary>
    /// Upper neighbor of this node.
    /// </summary>
    public INode Upper { get; }

    /// <summary>
    /// Lower neighbor of this node.
    /// </summary>
    public INode Lower { get; }
}
