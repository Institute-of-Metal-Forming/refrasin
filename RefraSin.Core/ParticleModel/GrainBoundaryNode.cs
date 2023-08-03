using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten, der Teil einer Korngrenze ist.
    /// </summary>
    public class GrainBoundaryNode : ContactNode<GrainBoundaryNode>, IGrainBoundaryNode
    {
        /// <inheritdoc />
        public GrainBoundaryNode((double phi, double r) coordinates) : base(coordinates) { }

        /// <inheritdoc />
        public override ToUpperToLower SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower(
            Particle.Material.SurfaceEnergy,
            Particle.Material.SurfaceEnergy
        );

        private ToUpperToLower? _surfaceEnergy;

        /// <inheritdoc />
        public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
            Particle.Material.SurfaceDiffusionCoefficient,
            Particle.Material.SurfaceDiffusionCoefficient
        );

        private ToUpperToLower? _surfaceDiffusionCoefficient;

    }
}