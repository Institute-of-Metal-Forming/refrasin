using System.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public class ParticleSurfaceEnumerable<TNode> : IEnumerable<TNode>
    where TNode : INode
{
    internal ParticleSurfaceEnumerable(
        IReadOnlyParticleSurface<TNode> surface,
        TNode startNode,
        Func<TNode, bool>? breakCondition,
        SurfaceIterationDirection direction
    )
    {
        Surface = surface;
        StartNode = startNode;
        BreakCondition = breakCondition;
        Direction = direction;
    }

    public SurfaceIterationDirection Direction { get; }
    public Func<TNode, bool>? BreakCondition { get; }
    public TNode StartNode { get; }
    public IReadOnlyParticleSurface<TNode> Surface { get; }

    /// <inheritdoc />
    public IEnumerator<TNode> GetEnumerator() =>
        new ParticleSurfaceEnumerator<TNode>(Surface, StartNode, BreakCondition, Direction);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class ParticleSurfaceEnumerator<TNode> : IEnumerator<TNode>
    where TNode : INode
{
    private TNode? _current;
    private readonly IReadOnlyParticleSurface<TNode> _surface;
    private readonly Func<TNode, bool>? _breakCondition;
    private readonly SurfaceIterationDirection _direction;
    private readonly TNode _startNode;

    internal ParticleSurfaceEnumerator(
        IReadOnlyParticleSurface<TNode> surface,
        TNode startNode,
        Func<TNode, bool>? breakCondition,
        SurfaceIterationDirection direction
    )
    {
        _surface = surface;
        _startNode = startNode;
        _breakCondition = breakCondition;
        _direction = direction;
        _current = default;
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        _current = _current is null ? _startNode : GetNextNode();

        if (_breakCondition is null)
            return true;
        return !_breakCondition(_current);
    }

    private TNode GetNextNode() =>
        _direction switch
        {
            SurfaceIterationDirection.Upward => _surface.UpperNeighborOf(Current),
            SurfaceIterationDirection.Downward => _surface.LowerNeighborOf(Current),
            _ => throw new ArgumentOutOfRangeException($"Invalid enum value {_direction}"),
        };

    /// <inheritdoc />
    public void Reset()
    {
        _current = default;
    }

    /// <inheritdoc />
    public TNode Current =>
        _current
        ?? throw new InvalidOperationException(
            "Enumerator in state before first element, call MoveNext() first."
        );

    /// <inheritdoc />
    object? IEnumerator.Current => Current;

    /// <inheritdoc />
    public void Dispose() { }
}

public enum SurfaceIterationDirection
{
    Upward = 0,
    Downward = 1,
}
