using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class VolumeBalanceConstraint : NodeEquationBase<NodeBase>
{
    /// <inheritdoc />
    public VolumeBalanceConstraint(SolutionState state, NodeBase node, StepVector step)
        : base(state, node, step) { }

    /// <inheritdoc />
    public override double Value()
    {
        var normalVolumeTerm = Node.VolumeGradient.Normal * Step.NormalDisplacement(Node);
        var tangentialVolumeTerm =
            Node is NeckNode
                ? Node.VolumeGradient.Tangential * Step.TangentialDisplacement(Node)
                : 0;
        var fluxTerm = Step.FluxToUpper(Node) - Step.FluxToUpper(Node.Lower);

        return normalVolumeTerm + tangentialVolumeTerm - fluxTerm;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        yield return (Map.NormalDisplacement(Node), Node.VolumeGradient.Normal);
        if (Node is NeckNode)
            yield return (Map.TangentialDisplacement(Node), Node.VolumeGradient.Tangential);
        yield return (Map.FluxToUpper(Node), -1);
        yield return (Map.FluxToUpper(Node.Lower), 1);
    }
}
