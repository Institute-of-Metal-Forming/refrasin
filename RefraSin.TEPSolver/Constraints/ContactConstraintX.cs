using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class ContactConstraintX : INodeContactItem, IConstraint
{
    private ContactConstraintX(ContactPair<NodeBase> nodeContact)
    {
        NodeContact = nodeContact;
    }

    public static INodeContactItem Create(ContactPair<NodeBase> nodeContact) =>
        new ContactConstraintX(nodeContact);

    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var thisTerm = GlobalNodeDisplacement(stepVector, NodeContact.First);
        var othersTerm = GlobalNodeDisplacement(stepVector, NodeContact.Second);

        return thisTerm - othersTerm;
    }

    private static double GlobalNodeDisplacement(StepVector stepVector, NodeBase node)
    {
        var byNormal =
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * stepVector.ItemValue<NormalDisplacement>(node);
        var byTangential = stepVector.StepVectorMap.HasItem<TangentialDisplacement>(node)
            ? Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusTangentAngle.ToLower
            ) * stepVector.ItemValue<TangentialDisplacement>(node)
            : 0;
        var byParticle = stepVector.ItemValue<ParticleDisplacementX>(node.Particle);

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
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * sign
        );
        if (stepVector.StepVectorMap.HasItem<TangentialDisplacement>(node))
            yield return (
                stepVector.StepVectorMap.ItemIndex<TangentialDisplacement>(node),
                Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * sign
            );
        yield return (
            stepVector.StepVectorMap.ItemIndex<ParticleDisplacementX>(node.Particle),
            sign
        );
    }

    public ContactPair<NodeBase> NodeContact { get; }

    public override string ToString() =>
        $"x contact constraint between {NodeContact.First} and {NodeContact.Second}";
}
