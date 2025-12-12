using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class PorePorosity(Pore pore) : IPoreItem, IStateVelocity, IFlux
{
    public double DrivingForce(StepVector stepVector)
    {
        var internalSinteringForce =
            -3
            * (2 * Pore.PoreMaterial.SurfaceEnergy - Pore.PoreMaterial.GrainBoundaryEnergy)
            / Pore.PoreMaterial.AverageParticleRadius
            * Pow(1 - Pore.Porosity, 2)
            * (2 * (1 - Pore.Porosity) - (1 - Pore.PoreMaterial.InitialPorosity))
            / (1 - Pore.PoreMaterial.InitialPorosity);

        return (Pore.PorousCompressionModulus * Pore.ElasticStrain + internalSinteringForce)
            / (1 - Pore.Porosity)
            * Pore.Volume;
    }

    public double DissipationFactor(StepVector stepVector) =>
        Pore.PorousVolumeViscosity / Pow(1 - Pore.Porosity, 2) * Pore.Volume;

    public Pore Pore { get; } = pore;

    public override string ToString() => $"porosity of {Pore}";
}
