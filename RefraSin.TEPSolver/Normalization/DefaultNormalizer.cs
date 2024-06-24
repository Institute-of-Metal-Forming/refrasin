using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using static System.Math;

namespace RefraSin.TEPSolver.Normalization;

public class DefaultNormalizer : INormalizer
{
    /// <inheritdoc />
    public INorm GetNorm(ISystemState referenceState, ISinteringStep sinteringStep) =>
        new Norm(
            referenceState, sinteringStep
        );

    private class Norm : INorm
    {
        public Norm(
            ISystemState referenceState, ISinteringStep sinteringStep
        )
        {
            var referenceParticle = referenceState.Particles[0];
            var referenceMaterial = sinteringStep.Materials.FirstOrDefault(m => m.Id == referenceParticle.MaterialId) ??
                                    throw new InvalidOperationException("Material of reference particle not present.");

            Length = referenceParticle.Nodes.Average(n => n.Coordinates.R);
            Area = Pow(Length, 2);
            Volume = Pow(Length, 3);
            
            Time = sinteringStep.GasConstant * sinteringStep.Temperature * Pow(Length, 4) /
                            (referenceMaterial.EquilibriumVacancyConcentration * referenceMaterial.MolarVolume *
                             referenceMaterial.Surface.DiffusionCoefficient * referenceMaterial.Surface.Energy);
            
            InterfaceEnergy = referenceMaterial.Surface.Energy;
            Energy = InterfaceEnergy * Area;
            
            Substance = Volume / referenceMaterial.MolarVolume;
            Mass = referenceMaterial.MolarMass * Substance;
            
            Temperature = Energy / (Substance * sinteringStep.GasConstant);

            DiffusionCoefficient = Volume / Time;
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

        /// <inheritdoc />
        public double DiffusionCoefficient { get; }


        /// <inheritdoc />
        public double InterfaceEnergy { get; }
    }
}