using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class TangentialDisplacementDerivative : NodeEquationBase<NodeBase>
{
    /// <inheritdoc />
    public TangentialDisplacementDerivative(SolutionState state, NodeBase node, StepVector step)
        : base(state, node, step) { }

    /// <inheritdoc />
    public override double Value()
    {
        var gibbsTerm = -Node.GibbsEnergyGradient.Tangential * (1 + Step.LambdaDissipation());
        var requiredConstraintsTerm = Node.VolumeGradient.Tangential * Step.LambdaVolume(Node);
        var contactTerm = Node is ContactNodeBase contactNode
            ? -contactNode.ContactDistanceGradient.Tangential * Step.LambdaContactDistance(Node)
                - contactNode.ContactDirectionGradient.Tangential
                    * Step.LambdaContactDirection(Node)
            : 0;

        return gibbsTerm + requiredConstraintsTerm + contactTerm;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        yield return (Map.LambdaDissipation(), -Node.GibbsEnergyGradient.Tangential);

        yield return (Map.LambdaVolume(Node), Node.VolumeGradient.Tangential);

        if (Node is ContactNodeBase contactNode)
        {
            yield return (
                Map.LambdaContactDistance(Node),
                -contactNode.ContactDistanceGradient.Tangential
            );
            yield return (
                Map.LambdaContactDirection(Node),
                -contactNode.ContactDirectionGradient.Tangential
            );
        }
    }
}
