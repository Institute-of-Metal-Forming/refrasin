using System.Collections;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel.Collections;

public class ReadOnlyContactCollection<TContact> : IReadOnlyContactCollection<TContact> where TContact : IEdge
{
    private TContact[] _contacts;
    private Dictionary<(Guid, Guid), int> _indices;
    private Dictionary<Guid, int[]>? _fromIndices;
    private Dictionary<Guid, int[]>? _toIndices;

    private ReadOnlyContactCollection()
    {
        _contacts = Array.Empty<TContact>();
        _indices = new Dictionary<(Guid, Guid), int>();
    }

    public ReadOnlyContactCollection(IEnumerable<TContact> contacts)
    {
        _contacts = contacts.ToArray();
        _indices = _contacts.Select((c, i) => ((c.From, c.To), i)).ToDictionary(t => t.Item1, t => t.i);
    }

    /// <inheritdoc />
    public IEnumerator<TContact> GetEnumerator() => ((IEnumerable<TContact>)_contacts).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _contacts.Length;

    /// <inheritdoc />
    public TContact this[int index] => _contacts[index];

    /// <inheritdoc />
    public TContact this[Guid from, Guid to] => _contacts[_indices[(from, to)]];

    /// <inheritdoc />
    public int IndexOf(Guid from, Guid to) => _indices[(from, to)];

    /// <inheritdoc />
    public bool Contains(Guid from, Guid to) => _indices.ContainsKey((from, to));

    /// <inheritdoc />
    public IEnumerable<TContact> From(Guid id) => FromIndices[id].Select(i => _contacts[i]);

    private Dictionary<Guid, int[]> FromIndices =>
        _fromIndices ??= _indices.GroupBy(i => i.Key.Item1, i => i.Value).ToDictionary(g => g.Key, g => g.ToArray());

    /// <inheritdoc />
    public IEnumerable<TContact> To(Guid id) => ToIndices[id].Select(i => _contacts[i]);

    private Dictionary<Guid, int[]> ToIndices =>
        _toIndices ??= _indices.GroupBy(i => i.Key.Item2, i => i.Value).ToDictionary(g => g.Key, g => g.ToArray());

    public static ReadOnlyContactCollection<TContact> Empty { get; } = new();
}