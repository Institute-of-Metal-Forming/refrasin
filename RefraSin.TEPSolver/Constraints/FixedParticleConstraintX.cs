using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class FixedParticleConstraintX : IParticleItem, IConstraint
{
    private FixedParticleConstraintX(Particle particle)
    {
        Particle = particle;
    }

    public static IParticleItem Create(Particle particle) => new FixedParticleConstraintX(particle);

    public double Residual(EquationSystem equationSystem, StepVector stepVector) =>
        stepVector.ItemValue<ParticleDisplacementX>(Particle);

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    ) => [(stepVector.StepVectorMap.ItemIndex<ParticleDisplacementX>(Particle), 1)];

    public Particle Particle { get; }

    public override string ToString() => $"x coordinate fixed for {Particle}";
}
