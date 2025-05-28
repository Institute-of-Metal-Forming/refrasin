using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class ParticleDisplacementX : IParticleItem, IStateVelocity
{
    private ParticleDisplacementX(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleItem Create(Particle particle) => new ParticleDisplacementX(particle);

    public double DrivingForce(StepVector stepVector) => 0;

    public Particle Particle { get; }

    public override string ToString() => $"x displacement of {Particle}";
}
