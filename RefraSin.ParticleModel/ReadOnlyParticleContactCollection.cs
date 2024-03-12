using System.Collections;

namespace RefraSin.ParticleModel;

public class ReadOnlyParticleContactCollection<TParticleContact> : IReadOnlyParticleContactCollection<TParticleContact> where TParticleContact : IParticleContact
{
    private TParticleContact[] _particleContacts;
    private Dictionary<(Guid, Guid), int> _particleContactIndices;

    private ReadOnlyParticleContactCollection()
    {
        _particleContacts = Array.Empty<TParticleContact>();
        _particleContactIndices = new Dictionary<(Guid, Guid), int>();
    }

    public ReadOnlyParticleContactCollection(IEnumerable<TParticleContact> particleContacts)
    {
        _particleContacts = particleContacts.ToArray();
        _particleContactIndices = _particleContacts.Select((c, i) => ((c.From.Id, c.To.Id), i)).ToDictionary(t => t.Item1, t => t.i);
    }

    /// <inheritdoc />
    public IEnumerator<TParticleContact> GetEnumerator() => ((IEnumerable<TParticleContact>)_particleContacts).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _particleContacts.Length;

    /// <inheritdoc />
    public TParticleContact this[int index] => _particleContacts[index];

    /// <inheritdoc />
    public TParticleContact this[Guid from, Guid to] => _particleContacts[_particleContactIndices[(from,to)]];

    /// <inheritdoc />
    public int IndexOf(Guid from, Guid to) => _particleContactIndices[(from,to)];

    /// <inheritdoc />
    public bool Contains(Guid from, Guid to) => _particleContactIndices.ContainsKey((from,to));

    public static ReadOnlyParticleContactCollection<TParticleContact> Empty { get; } = new();
}