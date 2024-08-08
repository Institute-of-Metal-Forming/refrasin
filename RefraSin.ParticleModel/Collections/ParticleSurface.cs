using System.Collections;
using RefraSin.Coordinates;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public class ParticleSurface<TNode> : IParticleSurface<TNode>
    where TNode : INode
{
    private List<TNode> _nodes;
    private Dictionary<Guid, int> _nodeIndices;
    private List<Angle> _nodeAngles;

    public ParticleSurface(IEnumerable<TNode> nodes)
    {
        _nodes = nodes.ToList();
        _nodeIndices = _nodes.Select((n, i) => (n.Id, i)).ToDictionary(t => t.Id, t => t.i);
        _nodeAngles = _nodes.Select(n => n.Coordinates.Phi).ToList();
    }

    /// <inheritdoc />
    public IEnumerator<TNode> GetEnumerator() => ((IEnumerable<TNode>)_nodes).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _nodes.Count;

    private int ReduceIndex(int index) => index >= 0 ? index % Count : Count + (index % Count);

    private TNode[] GetSlice(int start, int end)
    {
        var startIndex = ReduceIndex(start);
        var endIndex = ReduceIndex(end) + 1;

        return endIndex >= startIndex
            ? _nodes.Slice(startIndex, endIndex - startIndex).ToArray()
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
    public bool Contains(Guid nodeId) => _nodes.Any(n => n.Id == nodeId);

    /// <inheritdoc />
    public void InsertAbove(TNode position, TNode item) => InsertAbove(position.Id, item);

    /// <inheritdoc />
    public void InsertAbove(Guid position, TNode item) => InsertAbove(_nodeIndices[position], item);

    /// <inheritdoc />
    public void InsertAbove(int position, TNode item) => InsertBelow(position + 1, item);

    /// <inheritdoc />
    public void InsertBelow(TNode position, TNode item) => InsertBelow(position.Id, item);

    /// <inheritdoc />
    public void InsertBelow(Guid position, TNode item) => InsertBelow(_nodeIndices[position], item);

    /// <inheritdoc />
    public void InsertBelow(int position, TNode item)
    {
        _nodes.Insert(position, item);
        _nodeAngles.Insert(position, item.Coordinates.Phi);
        _nodeIndices.Add(item.Id, position);

        for (int i = position + 1; i < _nodes.Count; i++)
        {
            _nodeIndices[_nodes[i].Id] += 1;
        }
    }

    /// <inheritdoc />
    public void Remove(TNode item) => Remove(item.Id);

    /// <inheritdoc />
    public void Remove(Guid id) => Remove(_nodeIndices[id]);

    /// <inheritdoc />
    public void Remove(int index)
    {
        var id = _nodes[index].Id;
        _nodes.RemoveAt(index);
        _nodeAngles.RemoveAt(index);
        _nodeIndices.Remove(id);

        for (int i = index; i < _nodes.Count; i++)
        {
            _nodeIndices[_nodes[i].Id] -= 1;
        }
    }
}
