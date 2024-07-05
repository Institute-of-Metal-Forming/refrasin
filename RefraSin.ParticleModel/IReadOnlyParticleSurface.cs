namespace RefraSin.ParticleModel;

/// <summary>
/// An interface for collections of nodes, where items can be indexed by position and GUID.
/// Integer indices may be larger than the count of elements, which means counting from the beginning again (cyclic indexing).
/// Integer indices may be negative, which means counting from the end.
/// </summary>
public interface IReadOnlyParticleSurface<out TNode> : IReadOnlyNodeCollection<TNode> where TNode : INode
// IReadOnlyDictionary is not implemented, since this would break covariance
{
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
    /// Returns the node that is the first upper neighbor of the specified one in the surface.
    /// </summary>
    /// <param name="node"></param>
    public INode UpperNeighborOf(INode node) => this[IndexOf(node.Id) + 1];

    /// <summary>
    /// Returns the node that is the first lower neighbor of the specified one in the surface.
    /// </summary>
    /// <param name="node"></param>
    public INode LowerNeighborOf(INode node) => this[IndexOf(node.Id) - 1];
}