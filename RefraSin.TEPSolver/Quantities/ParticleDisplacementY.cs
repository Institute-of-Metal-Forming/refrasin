using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class ParticleDisplacementY : IParticleQuantity
{
    private ParticleDisplacementY(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleQuantity Create(SolutionState solutionState, Particle particle) =>
        new ParticleDisplacementY(particle);

    public double DrivingForce(StepVector stepVector) => 0;

    public Particle Particle { get; }
}
