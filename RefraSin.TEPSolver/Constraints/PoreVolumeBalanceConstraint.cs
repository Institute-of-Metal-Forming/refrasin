using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class PoreVolumeBalanceConstraint(Pore pore) : IPoreItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var volumeTerm = Pore
            .Nodes.Select(n =>
                (
                    x: GlobalNodeDisplacementX(stepVector, n),
                    y: GlobalNodeDisplacementY(stepVector, n)
                )
            )
            .Zip(Pore.VolumeDifferentials<Pore, NodeBase>())
            .Sum(t => t.First.x * t.Second.x + t.First.y * t.Second.y);
        var densityTerm =
            stepVector.ItemValue<PorePorosity>(Pore) / (1 - Pore.Porosity) * Pore.Volume;
        var elasticTerm = stepVector.ItemValue<PoreElasticStrain>(Pore) * Pore.Volume;
        var fluxTerm =
            Pore.Nodes.Where(stepVector.StepVectorMap.HasItem<FluxToPore>)
                .Sum(n =>
                    n.Particle.SubstanceProperties.Density
                    / Pore.PoreMaterial.Substance.Density
                    * stepVector.ItemValue<FluxToPore>(n)
                ) / (1 - Pore.Porosity);

        return densityTerm + elasticTerm + fluxTerm - volumeTerm;
    }

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        yield return (
            stepVector.StepVectorMap.ItemIndex<PorePorosity>(Pore),
            Pore.Volume / (1 - Pore.Porosity)
        );
        yield return (stepVector.StepVectorMap.ItemIndex<PoreElasticStrain>(Pore), Pore.Volume);

        foreach (
            var (n, volumeDifferential) in Pore.Nodes.Zip(
                Pore.VolumeDifferentials<Pore, NodeBase>()
            )
        )
        {
            foreach (var (i, x, y) in GlobalNodeDisplacementDerivatives(stepVector, n))
            {
                yield return (i, -(volumeDifferential.x * x + volumeDifferential.y * y));
            }

            if (stepVector.StepVectorMap.HasItem<FluxToPore>(n))
            {
                yield return (
                    stepVector.StepVectorMap.ItemIndex<FluxToPore>(n),
                    n.Particle.SubstanceProperties.Density
                        / Pore.PoreMaterial.Substance.Density
                        / (1 - Pore.Porosity)
                );
            }
        }
    }

    public Pore Pore { get; } = pore;

    public override string ToString() => $"volume balance constraint for {Pore}";

    private static double GlobalNodeDisplacementX(StepVector stepVector, NodeBase node)
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

    private static double GlobalNodeDisplacementY(StepVector stepVector, NodeBase node)
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

    private static IEnumerable<(int, double, double)> GlobalNodeDisplacementDerivatives(
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap.ItemIndex<NormalDisplacement>(node),
            -Cos(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            ),
            -Sin(
                node.Particle.RotationAngle + node.Coordinates.Phi + node.RadiusNormalAngle.ToLower
            )
        );
        if (stepVector.StepVectorMap.HasItem<TangentialDisplacement>(node))
            yield return (
                stepVector.StepVectorMap.ItemIndex<TangentialDisplacement>(node),
                Cos(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                ),
                Sin(
                    node.Particle.RotationAngle
                        + node.Coordinates.Phi
                        + node.RadiusTangentAngle.ToLower
                )
            );
        yield return (
            stepVector.StepVectorMap.ItemIndex<ParticleDisplacementY>(node.Particle),
            1,
            1
        );
    }
}
