using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class PoreDenseVolume(Pore pore) : IPoreItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector) =>
        -(
            Pore.PoreMaterial.SurfaceEnergy * Pore.SpecificSurfaceArea
            + Pore.PoreMaterial.GrainBoundaryEnergy * Pore.SpecificGrainBoundaryArea
        ) * Pore.ParticleCountDerivative;

    public Pore Pore { get; } = pore;

    public override string ToString() => $"dense volume of {Pore}";
}
