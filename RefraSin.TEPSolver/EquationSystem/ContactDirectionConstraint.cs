using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ContactDirectionConstraint : NodeEquationBase<ContactNodeBase>
{
    /// <inheritdoc />
    public ContactDirectionConstraint(ContactNodeBase node, StepVector step)
        : base(node, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Step.AngleDisplacement(Node.Contact)
        - Node.ContactDirectionGradient.Normal * Step.NormalDisplacement(Node)
        - Node.ContactedNode.ContactDirectionGradient.Normal
            * Step.NormalDisplacement(Node.ContactedNode)
        - (
            Node is NeckNode
                ? Node.ContactDirectionGradient.Tangential * Step.TangentialDisplacement(Node)
                    + Node.ContactedNode.ContactDirectionGradient.Tangential
                        * Step.TangentialDisplacement(Node.ContactedNode)
                : 0
        );

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        yield return (Map.AngleDisplacement(Node.Contact), 1.0);
        yield return (
            Map.RotationDisplacement(Node.Contact),
            -Node.ContactedNode.Coordinates.R
                / Node.Contact.Distance
                * Cos(Node.ContactedNode.AngleDistanceToContactDirection)
        );
        yield return (Map.NormalDisplacement(Node), -Node.ContactDirectionGradient.Normal);
        yield return (
            Map.NormalDisplacement(Node.ContactedNode),
            -Node.ContactedNode.ContactDirectionGradient.Normal
        );
        if (Node is NeckNode)
        {
            yield return (
                Map.TangentialDisplacement(Node),
                -Node.ContactDirectionGradient.Tangential
            );
            yield return (
                Map.TangentialDisplacement(Node.ContactedNode),
                -Node.ContactedNode.ContactDirectionGradient.Tangential
            );
        }
    }
}
