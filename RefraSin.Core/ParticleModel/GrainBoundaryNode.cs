using System;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;
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
        protected override void ClearCaches()
        {
            base.ClearCaches();
            _surfaceEnergy = null;
            _surfaceDiffusionCoefficient = null;
        }

        /// <inheritdoc cref="Node.ApplyState"/>
        public override void ApplyState(INode state)
        {
            base.ApplyState(state);

            _surfaceEnergy = state.SurfaceEnergy;
            _surfaceDiffusionCoefficient = state.SurfaceDiffusionCoefficient;
        }

        /// <inheritdoc />
        protected override void CheckState(INode state)
        {
            base.CheckState(state);

            if (state is not IGrainBoundaryNode)
                throw new ArgumentException($"The given state is no instance of {nameof(IGrainBoundaryNode)}", nameof(state));
        }
    }
}