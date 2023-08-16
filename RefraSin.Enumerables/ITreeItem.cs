namespace RefraSin.Enumerables;

/// <summary>
///     Interface for items of <see cref="Tree{TItem}" />.
/// </summary>
/// <typeparam name="TTreeItem">the implementing type</typeparam>
public interface ITreeItem<TTreeItem> where TTreeItem : class, ITreeItem<TTreeItem>
{
    /// <summary>
    ///     Parent of this element.
    /// </summary>
    public TTreeItem? Parent { get; set; }

    /// <summary>
    ///     Enumeration of child elements.
    /// </summary>
    public TreeChildrenCollection<TTreeItem> Children { get; }
}