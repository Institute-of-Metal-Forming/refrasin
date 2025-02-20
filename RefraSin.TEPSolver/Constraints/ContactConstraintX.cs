using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class ContactConstraintX : INodeConstraint
{
    private ContactConstraintX(ContactNodeBase node)
    {
        Node = node;
    }

    public static INodeConstraint Create(SolutionState solutionState, NodeBase node) =>
        new ContactConstraintX(
            node as ContactNodeBase
                ?? throw new ArgumentException($"Node given must be {typeof(ContactNodeBase)}.")
        );

    public double Residual(StepVector stepVector)
    {
        var thisTerm = GlobalNodeDisplacement(stepVector, Node);
        var othersTerm = GlobalNodeDisplacement(stepVector, Node.ContactedNode);

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
        DerivativesOf(stepVector, Node).Concat(DerivativesOf(stepVector, Node.ContactedNode));

    private static IEnumerable<(int, double)> DerivativesOf(StepVector stepVector, NodeBase node)
    {
        yield return (
            stepVector.StepVectorMap.QuantityIndex<NormalDisplacement>(node),
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            )
        );
        if (node is NeckNode)
            yield return (
                stepVector.StepVectorMap.QuantityIndex<TangentialDisplacement>(node),
                Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                )
            );
        yield return (
            stepVector.StepVectorMap.QuantityIndex<ParticleDisplacementX>(node.Particle),
            1
        );
    }

    public ContactNodeBase Node { get; }

    NodeBase INodeConstraint.Node => Node;
}
