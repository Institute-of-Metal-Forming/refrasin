using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class ParticleDisplacementX(Particle particle) : IParticleItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector) => 0;

    public Particle Particle { get; } = particle;

    public override string ToString() => $"x displacement of {Particle}";
}
