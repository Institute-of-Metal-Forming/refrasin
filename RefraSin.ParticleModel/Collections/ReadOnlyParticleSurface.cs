using System.Collections;
using RefraSin.Coordinates;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public class ReadOnlyParticleSurface<TNode> : IReadOnlyParticleSurface<TNode>
    where TNode : INode
{
    private TNode[] _nodes;
    private Dictionary<Guid, int> _nodeIndices;
    private List<Angle> _nodeAngles;

    public ReadOnlyParticleSurface(IEnumerable<TNode> nodes)
    {
        _nodes = nodes.ToArray();
        _nodeIndices = _nodes.Select((n, i) => (n.Id, i)).ToDictionary(t => t.Id, t => t.i);
        _nodeAngles = _nodes.Select(n => n.Coordinates.Phi).ToList();
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
    public IReadOnlyList<TNode> this[Guid start, Guid end] =>
        GetSlice(_nodeIndices[start], _nodeIndices[end]);

    /// <inheritdoc />
    public TNode NextLowerNodeFrom(Angle angle)
    {
        angle = angle.Reduce();
        var nextUpperIndex = _nodeAngles.BinarySearch(angle);
        return nextUpperIndex >= 0 ? _nodes[nextUpperIndex] : this[~nextUpperIndex - 1]; // this indexer to allow cyclic indexing
    }

    /// <inheritdoc />
    public TNode NextUpperNodeFrom(Angle angle)
    {
        angle = angle.Reduce();
        var nextUpperIndex = _nodeAngles.BinarySearch(angle);
        return nextUpperIndex >= 0 ? _nodes[nextUpperIndex] : this[~nextUpperIndex]; // this indexer to allow cyclic indexing
    }

    /// <inheritdoc />
    public int IndexOf(Guid nodeId) => _nodeIndices[nodeId];

    /// <inheritdoc />
    public bool Contains(Guid nodeId) => _nodeIndices.ContainsKey(nodeId);
}
