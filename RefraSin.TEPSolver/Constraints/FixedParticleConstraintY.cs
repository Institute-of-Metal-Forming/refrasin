using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class FixedParticleConstraintY(Particle particle) : IParticleItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector) =>
        stepVector.ItemValue<ParticleDisplacementY>(Particle);

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    ) => [(stepVector.StepVectorMap.ItemIndex<ParticleDisplacementY>(Particle), 1)];

    public Particle Particle { get; } = particle;

    public override string ToString() => $"y coordinate fixed for {Particle}";
}
