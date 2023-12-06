namespace RefraSin.ParticleModel;

/// <summary>
/// An interface for collections of nodes, where items can be indexed by position and GUID.
/// Integer indices may be larger than the count of elements, which means counting from the beginning again (cyclic indexing).
/// Integer indices may be negative, which means counting from the end.
/// </summary>
/// <typeparam name="TNode"></typeparam>
public interface IReadOnlyNodeCollection<out TNode> : IReadOnlyList<TNode> where TNode : INode
{
    /// <summary>
    /// Returns the node with the specified ID if present.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <exception cref="KeyNotFoundException">if a node with the specified ID is not present</exception>
    public TNode this[Guid nodeId] { get; }

    /// <summary>
    /// Returns a slice of nodes starting from <paramref name="start"/> to <paramref name="end"/> inclusively.
    /// </summary>
    /// <param name="start">start index</param>
    /// <param name="end">end index (inclusive)</param>
    public IReadOnlyList<TNode> this[int start, int end] { get; }

    /// <summary>
    /// Returns a slice of nodes starting from <paramref name="start"/> to <paramref name="end"/> inclusively.
    /// </summary>
    /// <param name="start">start ID</param>
    /// <param name="end">end ID (inclusive)</param>
    public IReadOnlyList<TNode> this[Guid start, Guid end] { get; }

    /// <summary>
    /// Returns the index of the specified node.
    /// </summary>
    /// <param name="nodeId">ID of the node to return the index for</param>
    /// <returns>the index in range 0 to <see cref="IReadOnlyNodeCollection{T}.Count"/>-1</returns>
    public int IndexOf(Guid nodeId);

    /// <summary>
    /// Indicates whether a node with the specified ID is contained in the collection.
    /// </summary>
    /// <param name="nodeId">the ID to test for</param>
    public bool Contains(Guid nodeId);
}