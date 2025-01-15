using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ContactConstraintX : NodeEquationBase<ContactNodeBase>
{
    /// <inheritdoc />
    public ContactConstraintX(SolutionState state, ContactNodeBase node, StepVector step)
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
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ) * Step.NormalDisplacement(node);
        var byTangential =
            node is NeckNode
                ? Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ) * Step.TangentialDisplacement(node)
                : 0;
        var byParticle = Step.ParticleDisplacementX(node.Particle);

        return byNormal + byTangential + byParticle;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative() =>
        DerivativesOf(Node).Concat(DerivativesOf(Node.ContactedNode));

    public IEnumerable<(int, double)> DerivativesOf(NodeBase node)
    {
        yield return (
            Map.NormalDisplacement(node),
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            )
        );
        if (node is NeckNode)
            yield return (
                Map.TangentialDisplacement(node),
                Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                )
            );
        yield return (Map.ParticleDisplacementX(node.Particle), 1);
    }
}
