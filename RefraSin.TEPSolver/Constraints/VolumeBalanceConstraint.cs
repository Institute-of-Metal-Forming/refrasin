using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class VolumeBalanceConstraint(NodeBase node) : INodeItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var normalVolumeTerm =
            Node.VolumeGradient.Normal * stepVector.ItemValue<NormalDisplacement>(Node);
        var tangentialVolumeTerm = stepVector.StepVectorMap.HasItem<TangentialDisplacement>(Node)
            ? Node.VolumeGradient.Tangential * stepVector.ItemValue<TangentialDisplacement>(Node)
            : 0;
        var fluxTerm =
            -stepVector.ItemValue<FluxToUpper>(Node)
            + stepVector.ItemValue<FluxToUpper>(Node.Lower);

        return normalVolumeTerm + tangentialVolumeTerm - fluxTerm;
    }

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        yield return (
            stepVector.StepVectorMap.ItemIndex<NormalDisplacement>(Node),
            Node.VolumeGradient.Normal
        );
        if (stepVector.StepVectorMap.HasItem<TangentialDisplacement>(Node))
            yield return (
                stepVector.StepVectorMap.ItemIndex<TangentialDisplacement>(Node),
                Node.VolumeGradient.Tangential
            );
        yield return (stepVector.StepVectorMap.ItemIndex<FluxToUpper>(Node), 1);
        yield return (stepVector.StepVectorMap.ItemIndex<FluxToUpper>(Node.Lower), -1);
    }

    public NodeBase Node { get; } = node;

    public override string ToString() => $"volume balance constraint for {Node}";
}
