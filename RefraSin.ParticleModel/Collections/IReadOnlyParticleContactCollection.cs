using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Collections;

/// <summary>
/// An interface for collections of particle contacts, where items can be indexed by position and GUIDs.
/// </summary>
/// <typeparam name="TParticleContact"></typeparam>
public interface IReadOnlyParticleContactCollection<out TParticleContact> : IReadOnlyList<TParticleContact> where TParticleContact : IParticleContact
// IReadOnlyDictionary is not implemented, since this would break covariance
{
    /// <summary>
    /// Returns the particle contact with the specified IDs if present.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <exception cref="KeyNotFoundException">if a particle contact with the specified IDs is not present</exception>
    public TParticleContact this[Guid from, Guid to] { get; }

    /// <summary>
    /// Returns the index of the specified particle contact.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>the index in range 0 to <see cref="IReadOnlyParticleSurface{TNode}.Count"/>-1, -1 if not found</returns>
    public int IndexOf(Guid from, Guid to);

    /// <summary>
    /// Indicates whether a particle contact with the specified IDs is contained in the collection.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public bool Contains(Guid from, Guid to);
}