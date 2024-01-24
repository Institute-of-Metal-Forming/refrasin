using System.Collections;

namespace RefraSin.ParticleModel;

public class ReadOnlyParticleCollection<TParticle> : IReadOnlyParticleCollection<TParticle> where TParticle : IParticle
{
    private TParticle[] _particles;
    private Dictionary<Guid, int> _particleIndices;

    private ReadOnlyParticleCollection()
    {
        _particles = Array.Empty<TParticle>();
        _particleIndices = new Dictionary<Guid, int>();
    }

    public ReadOnlyParticleCollection(IEnumerable<TParticle> particles)
    {
        _particles = particles.ToArray();
        _particleIndices = _particles.Select((n, i) => (n.Id, i)).ToDictionary(t => t.Id, t => t.i);
    }

    /// <inheritdoc />
    public IEnumerator<TParticle> GetEnumerator() => ((IEnumerable<TParticle>)_particles).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _particles.Length;

    /// <inheritdoc />
    public TParticle this[int index] => _particles[index];

    /// <inheritdoc />
    public TParticle this[Guid particleId] => _particles[_particleIndices[particleId]];

    /// <inheritdoc />
    public int IndexOf(Guid particleId) => _particleIndices[particleId];

    /// <inheritdoc />
    public bool Contains(Guid particleId) => _particleIndices.ContainsKey(particleId);

    /// <inheritdoc />
    public TParticle Root => _particles[0];

    public static ReadOnlyParticleCollection<TParticle> Empty { get; } = new();
}