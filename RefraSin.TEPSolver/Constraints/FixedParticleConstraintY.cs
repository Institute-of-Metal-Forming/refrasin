using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class FixedParticleConstraintY : IParticleConstraint
{
    private FixedParticleConstraintY(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleConstraint Create(SolutionState solutionState, Particle particle) =>
        new FixedParticleConstraintY(particle);

    public double Residual(StepVector stepVector) =>
        stepVector.QuantityValue<ParticleDisplacementY>(Particle);

    public IEnumerable<(int index, double value)> Derivatives(StepVector stepVector) =>
        [(stepVector.StepVectorMap.QuantityIndex<ParticleDisplacementY>(Particle), 1)];

    public Particle Particle { get; }
}
