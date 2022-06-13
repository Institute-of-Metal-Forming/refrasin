using System;
using System.Collections;
using System.Collections.Generic;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Enumerator für Oberflächenknoten. Iteriert von <see cref="First"/> bis einschließlich <see cref="Last"/> über jeweils <see cref="Node.Upper"/>.
    /// </summary>
    public class NodeEnumerator : IEnumerator<Node>
    {
        private Node? _current;

        private bool _endReached = false;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="first">erster Knoten</param>
        /// <param name="last">letzter Knoten</param>
        public NodeEnumerator(Node first, Node last)
        {
            First = first;
            Last = last;
        }

        /// <summary>
        /// Startknoten.
        /// </summary>
        public Node First { get; }

        /// <summary>
        /// Endknoten.
        /// </summary>
        public Node Last { get; }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_endReached) return false;

            if (_current == null)
            {
                _current = First;
                return true;
            }

            if (_current != Last)
            {
                var upper = _current.Upper;
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
        public Node Current =>
            _current ?? throw new InvalidOperationException("Invalid state of enumerator. Before start or after end of enumeration.");

        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose() { }
    }
}