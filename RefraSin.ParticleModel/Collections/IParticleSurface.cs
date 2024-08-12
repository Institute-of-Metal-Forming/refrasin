using RefraSin.Coordinates;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public interface IParticleSurface<TNode> : IReadOnlyParticleSurface<TNode>
    where TNode : INode
{
    /// <summary>
    /// Insert a node above a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node instance</param>
    /// <param name="item">the node to insert</param>
    public void InsertAbove(TNode position, TNode item);

    /// <summary>
    /// Insert a node above a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node ID</param>
    /// <param name="item">the node to insert</param>
    public void InsertAbove(Guid position, TNode item);

    /// <summary>
    /// Insert a node above a specified position.
    /// </summary>
    /// <param name="position">the position specified by a index</param>
    /// <param name="item">the node to insert</param>
    public void InsertAbove(int position, TNode item);

    /// <summary>
    /// Insert multiple nodes above a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node instance</param>
    /// <param name="items">the node to insert</param>
    public void InsertAbove(TNode position, IEnumerable<TNode> items);

    /// <summary>
    /// Insert multiple nodes above a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node ID</param>
    /// <param name="items">the node to insert</param>
    public void InsertAbove(Guid position, IEnumerable<TNode> items);

    /// <summary>
    /// Insert multiple nodes above a specified position.
    /// </summary>
    /// <param name="position">the position specified by a index</param>
    /// <param name="items">the node to insert</param>
    public void InsertAbove(int position, IEnumerable<TNode> items);

    /// <summary>
    /// Insert a node below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node instance</param>
    /// <param name="items">the node to insert</param>
    public void InsertBelow(TNode position, TNode items);

    /// <summary>
    /// Insert a node below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node ID</param>
    /// <param name="item">the node to insert</param>
    public void InsertBelow(Guid position, TNode item);

    /// <summary>
    /// Insert a node below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a index</param>
    /// <param name="item">the node to insert</param>
    public void InsertBelow(int position, TNode item);

    /// <summary>
    /// Insert multiple nodes below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node instance</param>
    /// <param name="items">the node to insert</param>
    public void InsertBelow(TNode position, IEnumerable<TNode> items);

    /// <summary>
    /// Insert multiple nodes below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node ID</param>
    /// <param name="items">the node to insert</param>
    public void InsertBelow(Guid position, IEnumerable<TNode> items);

    /// <summary>
    /// Insert multiple nodes below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a index</param>
    /// <param name="items">the node to insert</param>
    public void InsertBelow(int position, IEnumerable<TNode> items);

    /// <summary>
    /// Remove a specified instance.
    /// </summary>
    /// <param name="item">instance to remove</param>
    public void Remove(TNode item);

    /// <summary>
    /// Remove an item with the specified ID.
    /// </summary>
    /// <param name="id">ID of the item to remove</param>
    public void Remove(Guid id);

    /// <summary>
    /// Remove an item at a specified index.
    /// </summary>
    /// <param name="index">index of the item to remove</param>
    public void Remove(int index);

    /// <summary>
    /// Remove consecutive instances.
    /// </summary>
    /// <param name="start">start instance</param>
    /// <param name="end">end instance (inclusive)</param>
    public void Remove(TNode start, TNode end);

    /// <summary>
    /// Remove consecutive items with the specified IDs.
    /// </summary>
    /// <param name="start">start ID</param>
    /// <param name="end">end ID (inclusive)</param>
    public void Remove(Guid start, Guid end);

    /// <summary>
    /// Remove consecutive items at a specified range.
    /// </summary>
    /// <param name="start">start index</param>
    /// <param name="end">end index (inclusive)</param>
    public void Remove(int start, int end);
}
