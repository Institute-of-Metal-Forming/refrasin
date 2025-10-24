using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class PoreMassBalanceConstraint(Pore pore) : IPoreItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var densityTerm = Pore.Volume * stepVector.ItemValue<PoreDensity>(Pore);
        var volumeTerm =
            -Pore.RelativeDensity
            * (
                Pore.Nodes.Sum(n =>
                    n.VolumeGradient.Normal * stepVector.ItemValue<NormalDisplacement>(n)
                )
                + Pore.Nodes.Where(stepVector.StepVectorMap.HasItem<TangentialDisplacement>)
                    .Sum(n =>
                        n.VolumeGradient.Tangential
                        * stepVector.ItemValue<TangentialDisplacement>(n)
                    )
            );
        var fluxTerm = Pore
            .Nodes.Where(stepVector.StepVectorMap.HasItem<FluxToPore>)
            .Sum(n =>
                n.Particle.SubstanceProperties.Density
                / Pore.PoreMaterial.Substance.Density
                * stepVector.ItemValue<FluxToPore>(n)
            );

        return densityTerm + volumeTerm + fluxTerm;
    }

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        yield return (stepVector.StepVectorMap.ItemIndex<PoreDensity>(Pore), Pore.Volume);

        foreach (var n in Pore.Nodes)
        {
            yield return (
                stepVector.StepVectorMap.ItemIndex<NormalDisplacement>(n),
                -Pore.RelativeDensity * n.VolumeGradient.Normal
            );

            if (stepVector.StepVectorMap.HasItem<TangentialDisplacement>(n))
            {
                yield return (
                    stepVector.StepVectorMap.ItemIndex<TangentialDisplacement>(n),
                    -Pore.RelativeDensity * n.VolumeGradient.Tangential
                );
            }

            if (stepVector.StepVectorMap.HasItem<FluxToPore>(n))
            {
                yield return (
                    stepVector.StepVectorMap.ItemIndex<FluxToPore>(n),
                    n.Particle.SubstanceProperties.Density / Pore.PoreMaterial.Substance.Density
                );
            }
        }
    }

    public Pore Pore { get; } = pore;

    public override string ToString() => $"mass balance constraint for {Pore}";
}
