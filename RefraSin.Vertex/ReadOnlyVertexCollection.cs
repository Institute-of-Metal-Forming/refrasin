using System.Collections;

namespace RefraSin.Vertex;

public class ReadOnlyVertexCollection<TVertex> : IReadOnlyVertexCollection<TVertex>
    where TVertex : IVertex
{
    private TVertex[] _vertices;
    private Dictionary<Guid, int> _vertexIndices;

    private ReadOnlyVertexCollection()
    {
        _vertices = Array.Empty<TVertex>();
        _vertexIndices = new Dictionary<Guid, int>();
    }

    public ReadOnlyVertexCollection(IEnumerable<TVertex> vertexs)
    {
        _vertices = vertexs.ToArray();
        _vertexIndices = _vertices.Select((n, i) => (n.Id, i)).ToDictionary(t => t.Id, t => t.i);
    }

    /// <inheritdoc />
    public IEnumerator<TVertex> GetEnumerator() =>
        ((IEnumerable<TVertex>)_vertices).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _vertices.Length;

    /// <inheritdoc />
    public TVertex this[int vertexIndex] => _vertices[vertexIndex];

    /// <inheritdoc />
    public TVertex this[Guid vertexId] => _vertices[_vertexIndices[vertexId]];

    /// <inheritdoc />
    public int IndexOf(Guid vertexId) => _vertexIndices[vertexId];

    /// <inheritdoc />
    public bool Contains(Guid vertexId) => _vertexIndices.ContainsKey(vertexId);

    /// <summary>
    /// Returns an empty singleton instance.
    /// </summary>
    public static ReadOnlyVertexCollection<TVertex> Empty { get; } = new();
}
