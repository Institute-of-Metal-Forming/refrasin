using System.Collections;

namespace RefraSin.Enumerables;

public class FixedStack<T> : IReadOnlyCollection<T>
{
    private readonly LinkedList<T> _internalList;

    public FixedStack(int capacity)
    {
        _internalList = new LinkedList<T>();
        Capacity = capacity;
    }

    public int Capacity { get; }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _internalList.Count;

    public void Push(T item)
    {
        _internalList.AddFirst(item);

        if (_internalList.Count > Capacity)
            _internalList.RemoveLast();
    }

    public T Pop()
    {
        var item = _internalList.First.Value;
        _internalList.RemoveFirst();

        return item;
    }

    public T Head => _internalList.First.Value;
    public T Tail => _internalList.Last.Value;
}