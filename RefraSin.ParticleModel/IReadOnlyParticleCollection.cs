namespace RefraSin.ParticleModel;

/// <summary>
/// An interface for collections of particle, where items can be indexed by position and GUID.
/// </summary>
/// <typeparam name="TParticle"></typeparam>
public interface IReadOnlyParticleCollection<out TParticle> : IReadOnlyList<TParticle> where TParticle : IParticle
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
    /// <returns>the index in range 0 to <see cref="IReadOnlyNodeCollection{T}.Count"/>-1</returns>
    public int IndexOf(Guid particleId);
}