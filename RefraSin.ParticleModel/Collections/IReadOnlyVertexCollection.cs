using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

/// <summary>
/// An interface for collections of vertices, where items can be indexed by position and GUID.
/// </summary>
public interface IReadOnlyVertexCollection<out TVertex> : IReadOnlyList<TVertex>
    where TVertex : IVertex
// IReadOnlyDictionary is not implemented, since this would break covariance
{
    /// <summary>
    /// Returns the vertex with the specified ID if present.
    /// </summary>
    /// <param name="vertexId"></param>
    /// <exception cref="KeyNotFoundException">if a vertex with the specified ID is not present</exception>
    public TVertex this[Guid vertexId] { get; }

    /// <summary>
    /// Returns the index of the specified vertex.
    /// </summary>
    /// <param name="vertexId">ID of the vertex to return the index for</param>
    /// <exception cref="KeyNotFoundException">if a vertex with the specified ID is not present</exception>
    public int IndexOf(Guid vertexId);

    /// <summary>
    /// Returns the index of the specified vertex.
    /// </summary>
    /// <param name="vertex">vertex to return the index for</param>
    /// <returns>the index in range 0 to <see cref="IReadOnlyVertexCollection{TVertex}.Count"/>-1</returns>
    public int IndexOf(IVertex vertex) => IndexOf(vertex.Id);

    /// <summary>
    /// Indicates whether a vertex with the specified ID is contained in the collection.
    /// </summary>
    /// <param name="vertexId">the ID to test for</param>
    public bool Contains(Guid vertexId);
}
