using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RefraSin.Enumerables;

/// <summary>
///     Represents a ring enumeration where all elements are connected through neighborhood relations.
/// </summary>
/// <typeparam name="TRingItem">type of the items, must implement <see cref="IRingItem{TRingItem}" /></typeparam>
public partial class Ring<TRingItem> : ICollection<TRingItem>, IDisposable
    where TRingItem : class, IRingItem<TRingItem>
{
    /// <summary>
    ///     Creates a new ring.
    /// </summary>
    public Ring() { }

    /// <summary>
    ///     Creates a new ring and initializes it with the given elements.
    ///     The first element of <paramref name="enumerable" /> would be used as <see cref="Anchor" />.
    /// </summary>
    /// <param name="enumerable">enumeration of elements to initalizes with</param>
    /// <exception cref="ArgumentException">if <paramref name="enumerable" /> is empty</exception>
    public Ring(IEnumerable<TRingItem> enumerable)
    {
        using var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new ArgumentException($"{nameof(enumerable)} cannot be empty");

        SetInitialAnchor(enumerator.Current);

        while (enumerator.MoveNext())
            Add(enumerator.Current);
    }

    /// <summary>
    ///     Anchor element to start enumeration from.
    ///     Enumeration goes on upwards by getting subsequently <see cref="IRingItem{TRingItem}.Upper" />.
    /// </summary>
    public TRingItem? Anchor { get; private set; }

    /// <inheritdoc />
    public IEnumerator<TRingItem> GetEnumerator()
    {
        if (Anchor == null)
            return new Enumerator();
        return new Enumerator(Anchor, Anchor.Lower ?? Anchor);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Adds an item to this ring. Shortcut for <see cref="InsertBelow" /> with parameters (<see cref="Anchor" />,
    ///     <paramref name="item" />).
    /// </summary>
    /// <param name="item"></param>
    public void Add(TRingItem item)
    {
        if (Anchor != null)
            InsertBelow(Anchor, item);
        else
            SetInitialAnchor(item);
    }

    /// <summary>
    ///     Clears all members from this ring. References of the elements will be nulled.
    /// </summary>
    public void Clear()
    {
        using var enumerator = GetEnumerator();

        TRingItem last;
        if (enumerator.MoveNext())
            last = enumerator.Current;
        else
            return;

        var hasNext = true;
        while (hasNext)
        {
            hasNext = enumerator.MoveNext();

            last.Ring = null;
            last.Upper = null;
            last.Lower = null;

            last = enumerator.Current;
        }
    }

    /// <summary>
    ///     Determine if this ring contains a specific element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <remarks>
    ///     This method calls <c>item.IsMemberOf(this)</c> and additionally enumerates this ring until it finds the specified
    ///     instances.
    /// </remarks>
    public bool Contains(TRingItem item)
    {
        return item.IsMemberOf(this) && this.FirstOrDefault(i => ReferenceEquals(i, item)) != null;
    }

    /// <summary>
    ///     Copies this ring to an array. The <see cref="Anchor" /> will be the first element.
    /// </summary>
    /// <param name="array">array to copy to</param>
    /// <param name="arrayIndex">index to start</param>
    public void CopyTo(TRingItem[] array, int arrayIndex)
    {
        using var enumerator = GetEnumerator();

        for (var i = arrayIndex; ; i++)
        {
            if (!enumerator.MoveNext())
                break;
            array[i] = enumerator.Current;
        }
    }

    /// <summary>
    ///     Gets the count of elements.
    /// </summary>
    public int Count
    {
        get
        {
            using var enumerator = GetEnumerator();
            var count = 0;
            while (enumerator.MoveNext())
                count++;
            return count;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this ring is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    ///     Removes an item from this ring.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"><paramref name="item" /> is not part of this ring</exception>
    public bool Remove(TRingItem item)
    {
        if (!Contains(item))
            throw new InvalidOperationException($"{nameof(item)} is not part of this ring.");

        if (item.Upper != null)
            item.Upper.Lower = item.Lower;

        if (item.Lower != null)
            item.Lower.Upper = item.Upper;

        if (Anchor == item)
            Anchor =
                item.Upper ?? throw new InvalidOperationException("Last item cannot be removed.");

        item.Upper = null;
        item.Lower = null;
        item.Ring = null;

        return true;
    }

    /// <summary>
    ///     Returns a segment of this ring with the specified <paramref name="first" /> and <paramref name="last" /> elements.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     <paramref name="first" /> and/or <paramref name="last" /> are not member of
    ///     this ring or in invalid state
    /// </exception>
    public Segment GetSegment(TRingItem first, TRingItem last)
    {
        if (first.IsMemberOf(this) && last.IsMemberOf(this))
            return new Segment(first, last);
        throw new InvalidOperationException(
            $"Arguments {nameof(first)} and {nameof(last)} must be member of this ring."
        );
    }

    private void SetInitialAnchor(TRingItem anchor)
    {
        Anchor = anchor;
        anchor.Upper = anchor;
        anchor.Lower = anchor;
        anchor.Ring = this;
    }

    /// <summary>
    ///     Replaces an item in this ring.
    /// </summary>
    /// <param name="position">item to replace</param>
    /// <param name="replacement">replacement</param>
    /// <exception cref="InvalidOperationException"><paramref name="position" /> is not part of this ring</exception>
    public void Replace(TRingItem position, TRingItem replacement)
    {
        if (!Contains(position))
            throw new InvalidOperationException($"{nameof(position)} is not part of this ring.");
        if (replacement.IsRingMember())
            throw new InvalidOperationException(
                $"Cannot insert '{nameof(replacement)}', because '{nameof(replacement)}' is already part of another ring."
            );

        if (position.Upper != null)
        {
            position.Upper.Lower = replacement;
            replacement.Upper = position.Upper;
        }
        else
        {
            replacement.Upper = replacement;
        }

        if (position.Lower != null)
        {
            position.Lower.Upper = replacement;
            replacement.Lower = position.Lower;
        }
        else
        {
            replacement.Lower = replacement;
        }

        position.Upper = null;
        position.Lower = null;
        position.Ring = null;

        replacement.Ring = this;
        if (Anchor == position)
            Anchor = replacement;
    }

    /// <summary>
    ///     Inserts an item above another to this ring.
    /// </summary>
    /// <param name="position">item to insert above of</param>
    /// <param name="insertion">item to insert</param>
    /// <exception cref="InvalidOperationException"><paramref name="position" /> is not part of this ring</exception>
    public void InsertAbove(TRingItem position, TRingItem insertion)
    {
        if (!Contains(position))
            throw new InvalidOperationException($"{nameof(position)} is not part of this ring.");
        if (insertion.IsRingMember())
            throw new InvalidOperationException(
                $"Cannot insert '{nameof(insertion)}', because '{nameof(insertion)}' is already part of another ring."
            );

        if (position.Upper != null)
        {
            position.Upper.Lower = insertion;
            insertion.Upper = position.Upper;
        }
        else
        {
            position.Lower = insertion;
            insertion.Upper = position;
        }

        position.Upper = insertion;
        insertion.Lower = position;
        insertion.Ring = this;
    }

    /// <summary>
    ///     Inserts an item above another to this ring.
    /// </summary>
    /// <param name="position">item to insert below of</param>
    /// <param name="insertion">item to insert</param>
    /// <exception cref="InvalidOperationException"><paramref name="position" /> is not part of this ring</exception>
    public void InsertBelow(TRingItem position, TRingItem insertion)
    {
        if (!Contains(position))
            throw new InvalidOperationException($"{nameof(position)} is not part of this ring.");
        if (insertion.IsRingMember())
            throw new InvalidOperationException(
                $"Cannot insert '{nameof(insertion)}', because '{nameof(insertion)}' is already part of another ring."
            );

        if (position.Lower != null)
        {
            position.Lower.Upper = insertion;
            insertion.Lower = position.Lower;
        }
        else
        {
            position.Upper = insertion;
            insertion.Lower = position;
        }

        position.Lower = insertion;
        insertion.Upper = position;
        insertion.Ring = this;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Clear();
    }
}
