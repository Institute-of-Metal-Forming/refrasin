namespace RefraSin.ParticleModel;

/// <summary>
/// An interface for collections of nodes, where items can be indexed by position and GUID.
/// </summary>
public interface IReadOnlyNodeCollection<out TNode> : IReadOnlyList<TNode> where TNode : INode
// IReadOnlyDictionary is not implemented, since this would break covariance
{
    /// <summary>
    /// Returns the node with the specified ID if present.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <exception cref="KeyNotFoundException">if a node with the specified ID is not present</exception>
    public TNode this[Guid nodeId] { get; }

    /// <summary>
    /// Returns the index of the specified node.
    /// </summary>
    /// <param name="nodeId">ID of the node to return the index for</param>
    /// <returns>the index in range 0 to <see cref="IReadOnlyParticleSurface{TNode}.Count"/>-1</returns>
    public int IndexOf(Guid nodeId);

    /// <summary>
    /// Indicates whether a node with the specified ID is contained in the collection.
    /// </summary>
    /// <param name="nodeId">the ID to test for</param>
    public bool Contains(Guid nodeId);
}