using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ContactDistanceConstraint : NodeEquationBase<ContactNodeBase>
{
    /// <inheritdoc />
    public ContactDistanceConstraint(ContactNodeBase node, StepVector step)
        : base(node, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Step.RadialDisplacement(Node.Contact)
        - Node.ContactDistanceGradient.Normal * Step.NormalDisplacement(Node)
        - Node.ContactedNode.ContactDistanceGradient.Normal
            * Step.NormalDisplacement(Node.ContactedNode)
        - (
            Node is NeckNode
                ? Node.ContactDistanceGradient.Tangential * Step.TangentialDisplacement(Node)
                    + Node.ContactedNode.ContactDistanceGradient.Tangential
                        * Step.TangentialDisplacement(Node.ContactedNode)
                : 0
        );

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        yield return (Map.RadialDisplacement(Node.Contact), 1.0);
        yield return (Map.NormalDisplacement(Node), -Node.ContactDistanceGradient.Normal);
        yield return (
            Map.NormalDisplacement(Node.ContactedNode),
            -Node.ContactedNode.ContactDistanceGradient.Normal
        );

        if (Node is NeckNode)
        {
            yield return (
                Map.TangentialDisplacement(Node),
                -Node.ContactDistanceGradient.Tangential
            );
            yield return (
                Map.TangentialDisplacement(Node.ContactedNode),
                -Node.ContactedNode.ContactDistanceGradient.Tangential
            );
        }
    }
}
