using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class PorePorosity(Pore pore) : IPoreItem, IStateVelocity, IFlux
{
    public double DrivingForce(StepVector stepVector) =>
        -(
            Pore.PoreMaterial.SurfaceEnergy * Pore.SpecificSurfaceAreaDerivative
            + Pore.PoreMaterial.GrainBoundaryEnergy * Pore.SpecificGrainBoundaryAreaDerivative
        ) * Pore.ParticleCount;

    public double DissipationFactor(StepVector stepVector) =>
        Pore.PorousVolumeViscosity / Pow(1 - Pore.Porosity, 2) * Pore.Volume;

    public Pore Pore { get; } = pore;

    public override string ToString() => $"porosity of {Pore}";
}
