using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Collections;

/// <summary>
/// An interface for collections of particles, where items can be indexed by position and GUID.
/// </summary>
/// <typeparam name="TParticle"></typeparam>
/// <typeparam name="TNode"></typeparam>
public interface IReadOnlyParticleCollection<out TParticle, out TNode> : IReadOnlyList<TParticle> where TParticle : IParticle<TNode> where TNode : IParticleNode
// IReadOnlyDictionary is not implemented, since this would break covariance
{
    /// <summary>
    /// Returns the particle with the specified ID if present.
    /// </summary>
    /// <param name="particleId"></param>
    /// <exception cref="KeyNotFoundException">if a particle with the specified ID is not present</exception>
    public TParticle this[Guid particleId] { get; }

    /// <summary>
    /// Returns the index of the specified particle.
    /// </summary>
    /// <param name="particleId">ID of the particle to return the index for</param>
    /// <returns>the index in range 0 to <see cref="IReadOnlyParticleSurface{TNode}.Count"/>-1</returns>
    public int IndexOf(Guid particleId);

    /// <summary>
    /// Indicates whether a node with the specified ID is contained in the collection.
    /// </summary>
    /// <param name="particleId">the ID to test for</param>
    public bool Contains(Guid particleId);
    
    /// <summary>
    /// Gets the root particle.
    /// </summary>
    public TParticle Root { get; }
}