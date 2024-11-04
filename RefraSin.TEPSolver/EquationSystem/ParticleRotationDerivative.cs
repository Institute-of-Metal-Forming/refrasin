using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ParticleRotationDerivative : ContactEquationBase
{
    /// <inheritdoc />
    public ParticleRotationDerivative(ParticleContact contact, StepVector step)
        : base(contact, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Contact.FromNodes.Sum(n =>
            n.ContactedNode.Coordinates.R
                * Sin(n.ContactedNode.AngleDistanceToContactDirection)
                * Step.LambdaContactDistance(n)
            - n.ContactedNode.Coordinates.R
                / Contact.Distance
                * Cos(n.ContactedNode.AngleDistanceToContactDirection)
                * Step.LambdaContactDirection(n)
        );

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        foreach (var node in Contact.FromNodes)
        {
            yield return (
                Map.LambdaContactDistance(node),
                node.ContactedNode.Coordinates.R
                    * Sin(node.ContactedNode.AngleDistanceToContactDirection)
            );
            yield return (
                Map.LambdaContactDirection(node),
                -node.ContactedNode.Coordinates.R
                    / Contact.Distance
                    * Cos(node.ContactedNode.AngleDistanceToContactDirection)
            );
        }
    }
}
