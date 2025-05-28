using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class FixedParticleConstraintY : IParticleItem, IConstraint
{
    private FixedParticleConstraintY(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleItem Create(Particle particle) => new FixedParticleConstraintY(particle);

    public double Residual(EquationSystem equationSystem, StepVector stepVector) =>
        stepVector.ItemValue<ParticleDisplacementY>(Particle);

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    ) => [(stepVector.StepVectorMap.ItemIndex<ParticleDisplacementY>(Particle), 1)];

    public Particle Particle { get; }

    public override string ToString() => $"y coordinate fixed for {Particle}";
}
