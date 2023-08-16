using RefraSin.Coordinates;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten welcher am Punkt des Sinterhalse liegt. Vermittelt den Übergang zwischen freien Oberflächen und Korngrenzen.
    /// </summary>
    public class NeckNode : ContactNode<NeckNode>, INeckNode
    {
        /// <inheritdoc />
        public NeckNode((Angle phi, double r) coordinates) : base(coordinates) { }

        /// <inheritdoc />
        public Guid OppositeNeckNodeId { get; }

        public override ToUpperToLower SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower(
            Upper.SurfaceEnergy.ToLower,
            Lower.SurfaceEnergy.ToUpper
        );

        private ToUpperToLower? _surfaceEnergy;

        /// <inheritdoc />
        public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
            Upper.SurfaceDiffusionCoefficient.ToLower,
            Lower.SurfaceDiffusionCoefficient.ToUpper
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

            if (state is not INeckNode)
                throw new ArgumentException($"The given state is no instance of {nameof(INeckNode)}", nameof(state));
        }
    }
}