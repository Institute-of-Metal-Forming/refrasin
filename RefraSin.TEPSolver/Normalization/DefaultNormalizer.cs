using RefraSin.MaterialData;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using static System.Math;

namespace RefraSin.TEPSolver.Normalization;

public class DefaultNormalizer : INormalizer
{
    /// <inheritdoc />
    public INorm GetNorm(
        ISystemState referenceState,
        ISinteringConditions conditions,
        IEnumerable<IParticleMaterial> materials
    ) => new Norm(referenceState, conditions, materials);

    private class Norm : INorm
    {
        public Norm(
            ISystemState referenceState,
            ISinteringConditions sinteringStep,
            IEnumerable<IParticleMaterial> materials
        )
        {
            var referenceParticle = referenceState.Particles[0];
            var referenceMaterial =
                materials.FirstOrDefault(m => m.Id == referenceParticle.MaterialId)
                ?? throw new InvalidOperationException(
                    "Material of reference particle not present."
                );

            Length = referenceParticle.Nodes.Average(n => n.Coordinates.R);
            Area = Pow(Length, 2);
            Volume = Pow(Length, 3);

            Time =
                1e-3
                * sinteringStep.GasConstant
                * sinteringStep.Temperature
                * Pow(Length, 4)
                / (
                    referenceMaterial.Substance.MolarVolume
                    * referenceMaterial.Surface.DiffusionCoefficient
                    * referenceMaterial.Surface.Energy
                );

            Energy = referenceMaterial.Surface.Energy * Area;
            Mass = Energy / Area * Pow(Time, 2);
            Substance = Mass / referenceMaterial.Substance.MolarMass;
            Temperature = Energy / (Substance * sinteringStep.GasConstant);
        }

        /// <inheritdoc />
        public double Time { get; }

        /// <inheritdoc />
        public double Mass { get; }

        /// <inheritdoc />
        public double Length { get; }

        /// <inheritdoc />
        public double Temperature { get; }

        /// <inheritdoc />
        public double Substance { get; }

        /// <inheritdoc />
        public double Area { get; }

        /// <inheritdoc />
        public double Volume { get; }

        /// <inheritdoc />
        public double Energy { get; }
    }

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
