namespace RefraSin.Enumerables;

/// <summary>
///     Interface for items of <see cref="Ring{TRingItem}" />.
/// </summary>
/// <typeparam name="TRingItem">the implementing type</typeparam>
public interface IRingItem<TRingItem> where TRingItem : class, IRingItem<TRingItem>
{
    /// <summary>
    ///     Upper neighbor of this item.
    /// </summary>
    public TRingItem? Upper { get; set; }

    /// <summary>
    ///     Lower neighbor of this item.
    /// </summary>
    public TRingItem? Lower { get; set; }

    /// <summary>
    ///     Back reference to the parent <see cref="Ring{TRingItem}" />.
    /// </summary>
    public Ring<TRingItem>? Ring { get; set; }
}