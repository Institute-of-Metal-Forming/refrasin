using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class DissipationEqualityConstraint : GlobalEquationBase
{
    /// <inheritdoc />
    public DissipationEqualityConstraint(SolutionState state, StepVector step)
        : base(state, step) { }

    /// <inheritdoc />
    public override double Value()
    {
        var dissipationNormal = State
            .Nodes.Select(n => -n.GibbsEnergyGradient.Normal * Step.NormalDisplacement(n))
            .Sum();

        var dissipationTangential = State
            .Nodes.OfType<NeckNode>()
            .Select(n => -n.GibbsEnergyGradient.Tangential * Step.TangentialDisplacement(n))
            .Sum();

        var dissipationFunction = State
            .Nodes.Select(n =>
                n.Particle.VacancyVolumeEnergy
                * n.SurfaceDistance.ToUpper
                * Pow(Step.FluxToUpper(n), 2)
                / n.InterfaceDiffusionCoefficient.ToUpper
            )
            .Sum();

        return dissipationNormal + dissipationTangential - dissipationFunction;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        foreach (var n in State.Nodes)
        {
            yield return (Map.NormalDisplacement(n), -n.GibbsEnergyGradient.Normal);

            if (n is NeckNode)
                yield return (Map.TangentialDisplacement(n), -n.GibbsEnergyGradient.Tangential);
            yield return (
                Map.FluxToUpper(n),
                -2
                    * n.Particle.VacancyVolumeEnergy
                    * n.SurfaceDistance.ToUpper
                    / n.InterfaceDiffusionCoefficient.ToUpper
                    * Step.FluxToUpper(n)
            );
        }
    }
}
