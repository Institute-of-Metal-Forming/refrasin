using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class VolumeBalanceConstraint : INodeConstraint
{
    private VolumeBalanceConstraint(NodeBase node)
    {
        Node = node;
    }

    public static INodeConstraint Create(SolutionState solutionState, NodeBase node)
    {
        return new VolumeBalanceConstraint(node);
    }

    public double Residual(StepVector stepVector)
    {
        var normalVolumeTerm =
            Node.VolumeGradient.Normal * stepVector.QuantityValue<NormalDisplacement>(Node);
        var tangentialVolumeTerm =
            Node is NeckNode
                ? Node.VolumeGradient.Tangential
                    * stepVector.QuantityValue<TangentialDisplacement>(Node)
                : 0;
        var fluxTerm =
            stepVector.QuantityValue<FluxToUpper>(Node)
            - stepVector.QuantityValue<FluxToUpper>(Node.Lower);

        return normalVolumeTerm + tangentialVolumeTerm - fluxTerm;
    }

    public IEnumerable<(int index, double value)> Derivatives(StepVector stepVector)
    {
        yield return (
            stepVector.StepVectorMap.QuantityIndex<NormalDisplacement>(Node),
            Node.VolumeGradient.Normal
        );
        if (Node is NeckNode)
            yield return (
                stepVector.StepVectorMap.QuantityIndex<TangentialDisplacement>(Node),
                Node.VolumeGradient.Tangential
            );
        yield return (stepVector.StepVectorMap.QuantityIndex<FluxToUpper>(Node), -1);
        yield return (stepVector.StepVectorMap.QuantityIndex<FluxToUpper>(Node.Lower), 1);
    }

    public NodeBase Node { get; }
}
