using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class FixedParticleConstraintX(Particle particle) : IParticleItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector) =>
        stepVector.ItemValue<ParticleDisplacementX>(Particle);

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    ) => [(stepVector.StepVectorMap.ItemIndex<ParticleDisplacementX>(Particle), 1)];

    public Particle Particle { get; } = particle;

    public override string ToString() => $"x coordinate fixed for {Particle}";
}
