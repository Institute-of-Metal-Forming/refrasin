namespace RefraSin.Enumerables;

/// <summary>
///     Extension methods for <see cref="ITreeItem{TTreeItem}" />.
/// </summary>
public static class TreeItemExtensions
{
    /// <summary>
    ///     Test if this instance is parent of a given instance, this includes checking
    ///     <see cref="ITreeItem{TTreeItem}.Parent" /> property and containment in <see cref="ITreeItem{TTreeItem}.Children" />
    ///     .
    /// </summary>
    /// <param name="self">self</param>
    /// <param name="other">possible child</param>
    /// <returns></returns>
    public static bool IsParentOf<TTreeItem>(this TTreeItem self, TTreeItem other)
        where TTreeItem : class, ITreeItem<TTreeItem> =>
        other.Parent != null && other.Parent.Equals(self) && self.Children.Contains(other);

    /// <summary>
    ///     Test if this instance is child of a given instance, this includes checking
    ///     <see cref="ITreeItem{TTreeItem}.Parent" /> property and containment in <see cref="ITreeItem{TTreeItem}.Children" />
    ///     .
    /// </summary>
    /// <param name="self">self</param>
    /// <param name="other">possible parent</param>
    /// <returns></returns>
    public static bool IsChildOf<TTreeItem>(this TTreeItem self, TTreeItem other)
        where TTreeItem : class, ITreeItem<TTreeItem> =>
        self.Parent != null && self.Parent.Equals(other) && other.Children.Contains(self);

    /// <summary>
    ///     Add a child to this instance.
    /// </summary>
    /// <param name="self">self</param>
    /// <param name="item">child to add</param>
    /// <returns></returns>
    public static void AddChild<TTreeItem>(this TTreeItem self, TTreeItem item)
        where TTreeItem : class, ITreeItem<TTreeItem> => self.Children.Add(item);

    /// <summary>
    ///     Gets the tree with this item as root.
    /// </summary>
    /// <param name="self">self</param>
    /// <typeparam name="TTreeItem">type of the tree items</typeparam>
    /// <returns></returns>
    public static Tree<TTreeItem> GetTree<TTreeItem>(this TTreeItem self)
        where TTreeItem : class, ITreeItem<TTreeItem> => new(self);
}
