using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.TimeSteps;

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
            MaterialInterface.InterfaceEnergy,
            MaterialInterface.InterfaceEnergy
        );

        private ToUpperToLower? _surfaceEnergy;

        /// <inheritdoc />
        public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
            MaterialInterface.DiffusionCoefficient,
            MaterialInterface.DiffusionCoefficient
        );

        private ToUpperToLower? _surfaceDiffusionCoefficient;

        /// <inheritdoc />
        public override Node ApplyTimeStep(INodeTimeStep timeStep) => throw new System.NotImplementedException();
    }
}