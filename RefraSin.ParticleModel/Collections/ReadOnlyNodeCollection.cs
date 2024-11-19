using System.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public class ReadOnlyNodeCollection<TNode> : IReadOnlyNodeCollection<TNode>
    where TNode : INode
{
    private TNode[] _nodes;
    private Dictionary<Guid, int> _nodeIndices;

    private ReadOnlyNodeCollection()
    {
        _nodes = Array.Empty<TNode>();
        _nodeIndices = new Dictionary<Guid, int>();
    }

    public ReadOnlyNodeCollection(IEnumerable<TNode> nodes)
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

    /// <inheritdoc />
    public TNode this[int nodeIndex] => _nodes[nodeIndex];

    /// <inheritdoc />
    public TNode this[Guid nodeId] => _nodes[_nodeIndices[nodeId]];

    /// <inheritdoc />
    public int IndexOf(Guid nodeId) => _nodeIndices[nodeId];

    /// <inheritdoc />
    public bool Contains(Guid nodeId) => _nodeIndices.ContainsKey(nodeId);

    /// <summary>
    /// Returns an empty singleton instance.
    /// </summary>
    public static ReadOnlyNodeCollection<TNode> Empty { get; } = new();
}
