using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class ContactConstraintY(ContactPair<NodeBase> nodeContact) : INodeContactItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var thisTerm = GlobalNodeDisplacement(stepVector, NodeContact.First);
        var othersTerm = GlobalNodeDisplacement(stepVector, NodeContact.Second);

        return thisTerm - othersTerm;
    }

    private static double GlobalNodeDisplacement(StepVector stepVector, NodeBase node)
    {
        var byNormal =
            -Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * stepVector.ItemValue<NormalDisplacement>(node);
        var byTangential = stepVector.StepVectorMap.HasItem<TangentialDisplacement>(node)
            ? Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusTangentAngle.ToLower
            ) * stepVector.ItemValue<TangentialDisplacement>(node)
            : 0;
        var byParticle = stepVector.ItemValue<ParticleDisplacementY>(node.Particle);

        return byNormal + byTangential + byParticle;
    }

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    ) =>
        DerivativesOf(stepVector, NodeContact.First, 1)
            .Concat(DerivativesOf(stepVector, NodeContact.Second, -1));

    private static IEnumerable<(int, double)> DerivativesOf(
        StepVector stepVector,
        NodeBase node,
        int sign
    )
    {
        yield return (
            stepVector.StepVectorMap.ItemIndex<NormalDisplacement>(node),
            -Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * sign
        );
        if (stepVector.StepVectorMap.HasItem<TangentialDisplacement>(node))
            yield return (
                stepVector.StepVectorMap.ItemIndex<TangentialDisplacement>(node),
                Sin(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * sign
            );
        yield return (
            stepVector.StepVectorMap.ItemIndex<ParticleDisplacementY>(node.Particle),
            sign
        );
    }

    public ContactPair<NodeBase> NodeContact { get; } = nodeContact;

    public override string ToString() =>
        $"y contact constraint between {NodeContact.First} and {NodeContact.Second}";
}
