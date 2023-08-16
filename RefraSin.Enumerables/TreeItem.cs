using System.Collections.Generic;

namespace RefraSin.Enumerables;

/// <summary>
/// Standard implementation of <see cref="ITreeItem{TTreeItem}"/> for values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">type of the values</typeparam>
public class TreeItem<TValue> : ITreeItem<TreeItem<TValue>>
{
    /// <summary>
    /// Creates a new instance of <see cref="TreeItem{TValue}"/> and sets <see cref="Value"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">the value of this item</param>
    public TreeItem(TValue value)
    {
        Value = value;
        Children = new(this);
    }

    /// <summary>
    /// Creates a new instance of <see cref="TreeItem{TValue}"/>, sets <see cref="Value"/> to <paramref name="value"/> and adds the specified children.
    /// </summary>
    /// <param name="value">the value of this item</param>
    /// <param name="children">sequence of children</param>
    public TreeItem(TValue value, IEnumerable<TreeItem<TValue>> children)
    {
        Value = value;
        Children = new(this, children);
    }

    /// <inheritdoc />
    public TreeItem<TValue>? Parent { get; set; }

    /// <inheritdoc />
    public TreeChildrenCollection<TreeItem<TValue>> Children { get; }

    /// <summary>
    /// Gets or sets the value of this item.
    /// </summary>
    public TValue Value { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"TreeItem<{TypeNameEncoder.EncodeTypeName(typeof(TValue))}> Value={Value}";
}