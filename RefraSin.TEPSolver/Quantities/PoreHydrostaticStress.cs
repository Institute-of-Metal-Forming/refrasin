using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class PoreHydrostaticStress(Pore pore) : IPoreItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector) =>
        0 * -Pore.Volume / Pore.PorousCompressionModulus * Pore.HydrostaticStress;

    public Pore Pore { get; } = pore;

    public override string ToString() => $"hydrostatic stress of {Pore}";
}
