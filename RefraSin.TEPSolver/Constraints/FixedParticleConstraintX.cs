using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class FixedParticleConstraintX : IParticleConstraint
{
    private FixedParticleConstraintX(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleConstraint Create(Particle particle) =>
        new FixedParticleConstraintX(particle);

    public double Residual(StepVector stepVector) =>
        stepVector.QuantityValue<ParticleDisplacementX>(Particle);

    public IEnumerable<(int index, double value)> Derivatives(StepVector stepVector) =>
        [(stepVector.StepVectorMap.QuantityIndex<ParticleDisplacementX>(Particle), 1)];

    public Particle Particle { get; }
}
