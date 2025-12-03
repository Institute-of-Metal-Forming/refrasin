using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class PoreElasticStrain(Pore pore) : IPoreItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector) =>
        -Pore.Volume * Pore.PorousCompressionModulus * Pore.ElasticStrain;

    public Pore Pore { get; } = pore;

    public override string ToString() => $"hydrostatic stress of {Pore}";
}
