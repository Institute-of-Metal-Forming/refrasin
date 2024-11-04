using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ContactDirectionDerivative : ContactEquationBase
{
    /// <inheritdoc />
    public ContactDirectionDerivative(ParticleContact contact, StepVector step)
        : base(contact, step) { }

    /// <inheritdoc />
    public override double Value() => Contact.FromNodes.Sum(Step.LambdaContactDirection);

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative() =>
        Contact.FromNodes.Select(node => (Map.LambdaContactDistance(node), 1.0));
}
