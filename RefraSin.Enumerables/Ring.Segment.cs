using System.Collections;
using System.Collections.Generic;

namespace RefraSin.Enumerables;

public partial class Ring<TRingItem>
{
    /// <summary>
    /// Represents a segment of a <see cref="Ring{TRingItem}"/>.
    /// </summary>
    public class Segment : IEnumerable<TRingItem>
    {
        /// <summary>
        /// Creates a new instance with the given first and last elements.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        public Segment(TRingItem first, TRingItem last)
        {
            First = first;
            Last = last;
        }

        /// <summary>
        /// Gets the first element of this segment.
        /// </summary>
        public TRingItem First { get; }

        /// <summary>
        /// Gets the last element of this segment.
        /// </summary>
        public TRingItem Last { get; }

        /// <inheritdoc />
        public IEnumerator<TRingItem> GetEnumerator() => new Enumerator(First, Last);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
