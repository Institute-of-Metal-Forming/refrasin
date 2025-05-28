using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class ParticleDisplacementY : IParticleItem, IStateVelocity
{
    private ParticleDisplacementY(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleItem Create(Particle particle) => new ParticleDisplacementY(particle);

    public double DrivingForce(StepVector stepVector) => 0;

    public Particle Particle { get; }

    public override string ToString() => $"y displacement of {Particle}";
}
