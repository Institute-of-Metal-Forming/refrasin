using System;
using System.Collections;
using System.Collections.Generic;

namespace RefraSin.Enumerables;

public partial class Ring<TRingItem>
{
    /// <summary>
    ///     Enumerator over a <see cref="Ring{TRingItem}" />. Enumerates by subsequently getting
    ///     <see cref="IRingItem{TRingItem}.Upper" /> from <see cref="First" /> to <see cref="Last" />.
    /// </summary>
    public class Enumerator : IEnumerator<TRingItem>
    {
        private TRingItem? _current;

        private bool _endReached;
        private readonly TRingItem? _first;
        private readonly TRingItem? _last;

        /// <summary>
        ///     Creates a new enumerator with no elements.
        /// </summary>
        public Enumerator()
        {
            _endReached = true;
        }

        /// <summary>
        ///     Creates a new enumerator with the given boundary elements.
        /// </summary>
        /// <param name="first">first element to start enumeration from</param>
        /// <param name="last">last element to end enumeration with</param>
        public Enumerator(TRingItem first, TRingItem last)
        {
            _first = first;
            _last = last;
            _endReached = false;
        }

        /// <summary>
        ///     First element to start enumeration from.
        /// </summary>
        public TRingItem First => _first ?? throw new InvalidOperationException("Enumerator has no first element.");

        /// <summary>
        ///     Last element to end enumeration with.
        /// </summary>
        public TRingItem Last => _last ?? throw new InvalidOperationException("Enumerator has no last element.");

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_first == null) return false;
            if (_endReached) return false;

            if (_current == null)
            {
                _current = _first;
                return true;
            }

            if (!ReferenceEquals(_last, _current))
            {
                var upper = _current.Upper;
                if (!ReferenceEquals(upper?.Lower, _current))
                    throw new InvalidOperationException("Found invalid neighbor references while enumerating.");
                _current = upper;
                return true;
            }

            _endReached = true;
            _current = null;
            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _current = null;
            _endReached = false;
        }

        /// <inheritdoc />
        public TRingItem Current =>
            _current ?? throw new InvalidOperationException("Invalid state of enumerator. Before start or after end of enumeration.");

        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose() { }
    }
}