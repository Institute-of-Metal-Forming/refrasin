using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class ContactConstraintX : INodeContactConstraint
{
    private ContactConstraintX(ContactPair<NodeBase> nodeContact)
    {
        NodeContact = nodeContact;
    }

    public static INodeContactConstraint Create(ContactPair<NodeBase> nodeContact) =>
        new ContactConstraintX(nodeContact);

    public double Residual(StepVector stepVector)
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
            ) * stepVector.QuantityValue<NormalDisplacement>(node);
        var byTangential =
            node is NeckNode
                ? Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * stepVector.QuantityValue<TangentialDisplacement>(node)
                : 0;
        var byParticle = stepVector.QuantityValue<ParticleDisplacementX>(node.Particle);

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
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * sign
        );
        if (node is NeckNode)
            yield return (
                stepVector.StepVectorMap.QuantityIndex<TangentialDisplacement>(node),
                Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * sign
            );
        yield return (
            stepVector.StepVectorMap.QuantityIndex<ParticleDisplacementX>(node.Particle),
            sign
        );
    }

    public ContactPair<NodeBase> NodeContact { get; }
}
