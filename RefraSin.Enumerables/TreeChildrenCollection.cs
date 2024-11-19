using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RefraSin.Enumerables;

/// <summary>
///     Collection class for the children of <see cref="ITreeItem{TTreeItem}" />.
///     Ensures the correct setting of <see cref="ITreeItem{TTreeItem}.Parent" /> on adding to this collection.
/// </summary>
public class TreeChildrenCollection<TTreeItem> : IList<TTreeItem>
    where TTreeItem : class, ITreeItem<TTreeItem>
{
    private readonly List<TTreeItem> _children;
    private readonly TTreeItem _owner;

    /// <summary>
    ///     Creates a new children collection for the given parent.
    /// </summary>
    /// <param name="owner">the item containing this collection</param>
    public TreeChildrenCollection(TTreeItem owner)
    {
        _owner = owner;
        _children = new List<TTreeItem>(5);
    }

    /// <summary>
    ///     Creates a new children collection for the given parent with the specified elements.
    /// </summary>
    /// <param name="owner">the item containing this collection</param>
    /// <param name="children">sequence of children to add</param>
    public TreeChildrenCollection(TTreeItem owner, IEnumerable<TTreeItem> children)
    {
        _owner = owner;
        _children = children.ToList();
    }

    /// <inheritdoc />
    public IEnumerator<TTreeItem> GetEnumerator() => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(TTreeItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        _children.Add(item);
        item.Parent = _owner;
    }

    /// <inheritdoc />
    public void Clear()
    {
        foreach (var child in _children)
            child.Parent = null;

        _children.Clear();
    }

    /// <inheritdoc />
    public bool Contains(TTreeItem item) => _children.Contains(item);

    /// <inheritdoc />
    public void CopyTo(TTreeItem[] array, int arrayIndex) => _children.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(TTreeItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        item.Parent = null;
        return _children.Remove(item);
    }

    /// <inheritdoc />
    public int Count => _children.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf(TTreeItem item) => _children.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, TTreeItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        _children.Insert(index, item);
        item.Parent = _owner;
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        var item = _children[index];
        _children.RemoveAt(index);
        item.Parent = null;
    }

    /// <inheritdoc />
    public TTreeItem this[int index]
    {
        get => _children[index];
        set
        {
            var oldChild = _children[index];

            oldChild.Parent = null;
            value.Parent = _owner;
            _children[index] = value;
        }
    }
}
