using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ContactConstraintY : NodeEquationBase<ContactNodeBase>
{
    /// <inheritdoc />
    public ContactConstraintY(SolutionState state, ContactNodeBase node, StepVector step)
        : base(state, node, step) { }

    /// <inheritdoc />
    public override double Value()
    {
        var thisTerm = GlobalNodeDisplacement(Node);
        var othersTerm = GlobalNodeDisplacement(Node.ContactedNode);

        return thisTerm - othersTerm;
    }

    public double GlobalNodeDisplacement(NodeBase node)
    {
        var byNormal =
            -Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * Step.NormalDisplacement(node);
        var byTangential =
            node is NeckNode
                ? Sin(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * Step.TangentialDisplacement(node)
                : 0;
        var byParticle = Step.ParticleDisplacementY(node.Particle);

        return byNormal + byTangential + byParticle;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative() =>
        DerivativesOf(Node).Concat(DerivativesOf(Node.ContactedNode));

    public IEnumerable<(int, double)> DerivativesOf(NodeBase node)
    {
        yield return (
            Map.NormalDisplacement(node),
            -Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            )
        );
        if (node is NeckNode)
            yield return (
                Map.TangentialDisplacement(node),
                Sin(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                )
            );
        yield return (Map.ParticleDisplacementY(node.Particle), 1);
    }
}
