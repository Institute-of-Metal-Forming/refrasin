using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class NormalDisplacementDerivative : NodeEquationBase<NodeBase>
{
    /// <inheritdoc />
    public NormalDisplacementDerivative(SolutionState state, NodeBase node, StepVector step)
        : base(state, node, step) { }

    /// <inheritdoc />
    public override double Value()
    {
        var gibbsTerm = -Node.GibbsEnergyGradient.Normal * (1 + Step.LambdaDissipation());
        var requiredConstraintsTerm = Node.VolumeGradient.Normal * Step.LambdaVolume(Node);

        double contactTerm = 0;

        if (Node is ContactNodeBase contactNode)
        {
            contactTerm =
                -contactNode.ContactDistanceGradient.Normal * Step.LambdaContactX(contactNode)
                - contactNode.ContactDirectionGradient.Normal * Step.LambdaContactY(contactNode);
        }

        return gibbsTerm + requiredConstraintsTerm + contactTerm;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        yield return (Map.LambdaDissipation(), -Node.GibbsEnergyGradient.Normal);

        yield return (Map.LambdaVolume(Node), Node.VolumeGradient.Normal);

        if (Node is ContactNodeBase contactNode)
        {
            yield return (
                Map.LambdaContactX(contactNode),
                -contactNode.ContactDistanceGradient.Normal
            );
            yield return (
                Map.LambdaContactY(contactNode),
                -contactNode.ContactDirectionGradient.Normal
            );
        }
    }
}
