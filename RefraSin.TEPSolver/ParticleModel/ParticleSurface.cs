using RefraSin.Coordinates;
using RefraSin.Enumerables;

namespace RefraSin.TEPSolver.ParticleModel
{
    /// <summary>
    /// Stellt die Oberfläche eines Pulverpartikels als Aufzählung von Oberflächenknoten dar.
    /// </summary>
    public class ParticleSurface : Ring<Node>
    {
        /// <inheritdoc />
        public ParticleSurface(Particle particle)
        {
            Particle = particle;
        }

        public Particle Particle { get; }

        public void AddNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        /// <summary>
        /// Bestimmt die beiden einem Winkel nächstgelegenen Oberflächenknoten.
        /// </summary>
        /// <param name="angle">Winkel</param>
        /// <returns></returns>
        public (Node Upper, Node Lower) GetNearestNodesToAngle(Angle angle)
        {
            var nodes = this.OrderBy(k => Angle.ReduceRadians(k.Coordinates.Phi.Radians, Angle.ReductionDomain.AllPositive)).ToArray();
            var upper = nodes.FirstOrDefault(k => k.Coordinates.Phi.Radians > angle.Radians) ?? nodes.First();
            var lower = upper.Lower;
            return (upper, lower);
        }

        /// <summary>
        /// Berechnet den zwischen den angrenzenden Knoten interpolierten Radius an einer bestimmten Winkelkoordinate.
        /// </summary>
        /// <param name="angle">Winkel</param>
        /// <returns></returns>
        public double InterpolatedRadius(Angle angle)
        {
            var (upper, lower) = GetNearestNodesToAngle(angle);
            return lower.Coordinates.R + (upper.Coordinates.R - lower.Coordinates.R) /
                (upper.Coordinates.Phi - lower.Coordinates.Phi).Reduce().Radians * (angle - lower.Coordinates.Phi).Reduce().Radians;
        }
    }
}