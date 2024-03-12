namespace RefraSin.ParticleModel;

public interface IParticleSurface<TNode> : IReadOnlyParticleSurface<TNode> where TNode : INode
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
    /// Insert a node below a specified position.
    /// </summary>
    /// <param name="position">the position specified by a node instance</param>
    /// <param name="item">the node to insert</param>
    public void InsertBelow(TNode position, TNode item);

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
}