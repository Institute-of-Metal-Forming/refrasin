using IMF.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten, der Teil einer Korngrenze ist.
    /// </summary>
    public class GrainBoundaryNode : ContactNode<GrainBoundaryNode>, IGrainBoundaryNode
    {
        private ToUpperToLower? _diffusionCoefficient;

        /// <inheritdoc />
        public GrainBoundaryNode(Particle particle, PolarPoint coordinates) : base(particle, coordinates) { }

        public override double InterfaceEnergy => MaterialInterface.InterfaceEnergy;

        public override double Transfer => MaterialInterface.TransferCoefficient * (VacancyConcentration - ContactedNode.VacancyConcentration)
            * SurfaceDistance.Sum / 2;

        public override ToUpperToLower DiffusionCoefficient => _diffusionCoefficient ??=
            new ToUpperToLower(MaterialInterface.DiffusionCoefficient, MaterialInterface.DiffusionCoefficient);

        /// <summary>
        /// Berechnet die ungefähre Kontaktspannung für ein gegebenes Überhangvolumen.
        /// </summary>
        /// <param name="failVolume">Überhangsvolumen</param>
        /// <param name="timeStepWidth">Zeitschrittweite</param>
        /// <returns></returns>
        public double CalculateContactStressChangeFromFailVolume(double failVolume, double timeStepWidth)
            => (SurfaceDistance.ToUpper * SurfaceDistance.ToLower * Particle.Process.UniversalGasConstant * Particle.Process.Temperature) /
                (MaterialInterface.DiffusionCoefficient * SurfaceDistance.Sum *
                 Particle.Material.MolarVolume * Particle.Material.ThermalVacancyConcentration * timeStepWidth) * failVolume / 2;

        public override void Disconnect()
        {
            _diffusionCoefficient = null;
            base.Disconnect();
        }
    }
}