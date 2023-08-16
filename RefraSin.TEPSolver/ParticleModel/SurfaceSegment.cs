using System.Collections;

namespace RefraSin.TEPSolver.ParticleModel
{
    /// <summary>
    /// Stellt einen Abschnitt einer Partikeloberfläche dar. Kann zum iterieren über die Knoten des Abschnitts genutzt werden.
    /// </summary>
    public class SurfaceSegment : IEnumerable<Node>
    {
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="lowerEnd">Knoten, der das untere Ende des Abschnitts definiert</param>
        /// <param name="upperEnd">Knoten, der das obere Ende des Abschnitts definiert</param>
        public SurfaceSegment(Node lowerEnd, Node upperEnd)
        {
            LowerEnd = lowerEnd;
            UpperEnd = upperEnd;
        }

        /// <summary>
        /// Knoten, der das untere Ende des Abschnitts definiert
        /// </summary>
        public Node LowerEnd { get; }

        /// <summary>
        /// Knoten, der das obere Ende des Abschnitts definiert
        /// </summary>
        public Node UpperEnd { get; }

        /// <inheritdoc />
        public IEnumerator<Node> GetEnumerator()
        {
            return new NodeEnumerator(LowerEnd, UpperEnd);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}