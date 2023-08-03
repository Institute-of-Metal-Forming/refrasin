using IMF.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;
using RefraSin.Core.Solver;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten, der Teil einer freien Oberfläche ist.
    /// </summary>
    public class SurfaceNode : Node, ISurfaceNode
    {
        /// <inheritdoc />
        public SurfaceNode(Particle particle, PolarPoint coordinates) : base(particle, coordinates)
        {
            DiffusionCoefficient = new ToUpperToLower(Particle.Material.SurfaceDiffusionCoefficient,
                Particle.Material.SurfaceDiffusionCoefficient);
        }

        public override double InterfaceEnergy => Particle.Material.SurfaceEnergy;

        public override ToUpperToLower DiffusionCoefficient { get; }

        public void RemoveSelfIfNeighborsToClose(ISinteringSolverSession session)
        {
            var limit = session.SolverOptions.DiscretizationWidth * session.SolverOptions.RemeshingDistanceDeletionLimit;
            if (SurfaceDistance.ToUpper < limit && SurfaceDistance.ToLower < limit)
                Remove();
        }
    }
}