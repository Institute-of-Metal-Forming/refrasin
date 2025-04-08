using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class ContactConstraintY : INodeContactConstraint
{
    private ContactConstraintY(ContactPair<NodeBase> nodeContact)
    {
        NodeContact = nodeContact;
    }

    public static INodeContactConstraint Create(ContactPair<NodeBase> nodeContact) =>
        new ContactConstraintY(nodeContact);

    public double Residual(StepVector stepVector)
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
            ) * stepVector.QuantityValue<NormalDisplacement>(node);
        var byTangential =
            node is NeckNode
                ? Sin(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * stepVector.QuantityValue<TangentialDisplacement>(node)
                : 0;
        var byParticle = stepVector.QuantityValue<ParticleDisplacementY>(node.Particle);

        return byNormal + byTangential + byParticle;
    }

    public IEnumerable<(int index, double value)> Derivatives(StepVector stepVector) =>
        DerivativesOf(stepVector, NodeContact.First, 1)
            .Concat(DerivativesOf(stepVector, NodeContact.Second, -1));

    private static IEnumerable<(int, double)> DerivativesOf(
        StepVector stepVector,
        NodeBase node,
        int sign
    )
    {
        yield return (
            stepVector.StepVectorMap.QuantityIndex<NormalDisplacement>(node),
            -Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * sign
        );
        if (node is NeckNode)
            yield return (
                stepVector.StepVectorMap.QuantityIndex<TangentialDisplacement>(node),
                Sin(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * sign
            );
        yield return (
            stepVector.StepVectorMap.QuantityIndex<ParticleDisplacementY>(node.Particle),
            sign
        );
    }

    public ContactPair<NodeBase> NodeContact { get; }

    public override string ToString() =>
        $"y contact constraint between {NodeContact.First} and {NodeContact.Second}";
}
