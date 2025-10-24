using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class PoreDensity(Pore pore) : IPoreItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector) =>
        -Pore.Pressure / Pore.RelativeDensity * Pore.Volume;

    public Pore Pore { get; } = pore;

    public override string ToString() => $"density of {Pore}";
}
