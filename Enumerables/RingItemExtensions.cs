using System;

namespace RefraSin.Enumerables;

/// <summary>
///     Extension methods for <see cref="IRingItem{TRingItem}" />.
/// </summary>
public static class RingItemExtensions
{
    /// <summary>
    ///     Removes this item from the ring.
    /// </summary>
    /// <param name="self"></param>
    public static void Remove<TRingItem>(this TRingItem self) where TRingItem : class, IRingItem<TRingItem>
    {
        if (self.Ring != null)
            self.Ring.Remove(self);
        else
            throw new PropertyNotSetException(nameof(self.Ring), nameof(IRingItem<TRingItem>));
    }

    /// <summary>
    ///     Replaces this item in the ring by another.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="replacement"></param>
    public static void Replace<TRingItem>(this TRingItem self, TRingItem replacement) where TRingItem : class, IRingItem<TRingItem>
    {
        if (self.Ring != null)
            self.Ring.Replace(self, replacement);
        else
            throw new PropertyNotSetException(nameof(self.Ring), nameof(IRingItem<TRingItem>));
    }

    /// <summary>
    ///     Inserts a new item above this.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="insertion"></param>
    public static void InsertAbove<TRingItem>(this TRingItem self, TRingItem insertion) where TRingItem : class, IRingItem<TRingItem>
    {
        if (self.Ring != null)
            self.Ring.InsertAbove(self, insertion);
        else
            throw new PropertyNotSetException(nameof(self.Ring), nameof(IRingItem<TRingItem>));
    }

    /// <summary>
    ///     Inserts a new item below this.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="insertion"></param>
    public static void InsertBelow<TRingItem>(this TRingItem self, TRingItem insertion) where TRingItem : class, IRingItem<TRingItem>
    {
        if (self.Ring != null)
            self.Ring.InsertBelow(self, insertion);
        else
            throw new PropertyNotSetException(nameof(self.Ring), nameof(IRingItem<TRingItem>));
    }

    /// <summary>
    ///     Test if this instance is a member of some <see cref="Ring{TRingItem}" />.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     invalid combination of <see cref="IRingItem{TRingItem}.Ring" />, <see cref="IRingItem{TRingItem}.Upper" /> and
    ///     <see cref="IRingItem{TRingItem}.Lower" /> states
    /// </exception>
    public static bool IsRingMember<TRingItem>(this TRingItem self) where TRingItem : class, IRingItem<TRingItem>
    {
        if (self.Ring != null)
        {
            if (self.Upper != null && self.Lower != null && self.Upper.Ring == self.Ring && self.Lower.Ring == self.Ring) return true;

            throw new InvalidOperationException("This object is member of a ring, but has no valid neighbors. Something seems broken.");
        }

        if (self.Upper == null && self.Lower == null) return false;

        throw new InvalidOperationException("This object is no member of a ring, but has neighbors. Something seems broken.");
    }

    /// <summary>
    ///     Test if this instance is a member of the <see cref="Ring{TRingItem}" /> <paramref name="ring"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     invalid combination of <see cref="IRingItem{TRingItem}.Ring" />, <see cref="IRingItem{TRingItem}.Upper" /> and
    ///     <see cref="IRingItem{TRingItem}.Lower" /> states
    /// </exception>
    /// <remarks>
    /// This method does not enumerate the ring. It checks if the <see cref="IRingItem{TRingItem}.Ring" /> properties of <paramref name="self"/> and its neighbors equals <paramref name="ring"/>.
    /// </remarks>
    public static bool IsMemberOf<TRingItem>(this TRingItem self, Ring<TRingItem> ring) where TRingItem : class, IRingItem<TRingItem> =>
        self.IsRingMember() && ReferenceEquals(self.Ring, ring);
}