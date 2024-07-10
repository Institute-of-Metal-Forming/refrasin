using System.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public class ReadOnlyParticleSurface<TNode> : IReadOnlyParticleSurface<TNode> where TNode : INode
{
    private TNode[] _nodes;
    private Dictionary<Guid, int> _nodeIndices;

    private ReadOnlyParticleSurface()
    {
        _nodes = Array.Empty<TNode>();
        _nodeIndices = new Dictionary<Guid, int>();
    }

    public ReadOnlyParticleSurface(IEnumerable<TNode> nodes)
    {
        _nodes = nodes.ToArray();
        _nodeIndices = _nodes.Select((n, i) => (n.Id, i)).ToDictionary(t => t.Id, t => t.i);
    }

    /// <inheritdoc />
    public IEnumerator<TNode> GetEnumerator() => ((IEnumerable<TNode>)_nodes).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _nodes.Length;

    private int ReduceIndex(int index) => index >= 0 ? index % Count : Count + (index % Count);

    private TNode[] GetSlice(int start, int end)
    {
        var startIndex = ReduceIndex(start);
        var endIndex = ReduceIndex(end) + 1;

        return endIndex >= startIndex
            ? _nodes[startIndex..endIndex]
            : _nodes[startIndex..].Concat(_nodes[..endIndex]).ToArray();
    }

    /// <inheritdoc />
    public TNode this[int nodeIndex] => _nodes[ReduceIndex(nodeIndex)];

    /// <inheritdoc />
    public TNode this[Guid nodeId] => _nodes[_nodeIndices[nodeId]];

    /// <inheritdoc />
    public IReadOnlyList<TNode> this[int start, int end] => GetSlice(start, end);

    /// <inheritdoc />
    public IReadOnlyList<TNode> this[Guid start, Guid end] => GetSlice(_nodeIndices[start], _nodeIndices[end]);

    /// <inheritdoc />
    public int IndexOf(Guid nodeId) => _nodeIndices[nodeId];

    /// <inheritdoc />
    public bool Contains(Guid nodeId) => _nodeIndices.ContainsKey(nodeId);

    /// <summary>
    /// Returns an empty singleton instance.
    /// </summary>
    public static ReadOnlyParticleSurface<TNode> Empty { get; } = new();
}