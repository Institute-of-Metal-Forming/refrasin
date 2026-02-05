using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class PoreVolumeBalanceFluxConstraint(Pore pore) : IPoreItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector) =>
        stepVector.ItemValue<PoreDenseVolume>(Pore)
        - Pore.Nodes.Where(stepVector.StepVectorMap.HasItem<FluxToPore>)
            .Sum(n =>
                n.Particle.SubstanceProperties.Density
                / Pore.PoreMaterial.Substance.Density
                * stepVector.ItemValue<FluxToPore>(n)
            );

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        yield return (stepVector.StepVectorMap.ItemIndex<PoreDenseVolume>(Pore), 1);

        foreach (var n in Pore.Nodes)
        {
            if (stepVector.StepVectorMap.HasItem<FluxToPore>(n))
            {
                yield return (
                    stepVector.StepVectorMap.ItemIndex<FluxToPore>(n),
                    -n.Particle.SubstanceProperties.Density
                        / Pore.PoreMaterial.Substance.Density
                        / (1 - Pore.Porosity)
                );
            }
        }
    }

    public Pore Pore { get; } = pore;

    public override string ToString() => $"volume balance flux constraint for {Pore}";
}
