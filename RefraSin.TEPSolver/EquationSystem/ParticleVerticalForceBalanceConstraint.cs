using RefraSin.Coordinates;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ParticleVerticalForceBalanceConstraint : ParticleEquationBase
{
    /// <inheritdoc />
    public ParticleVerticalForceBalanceConstraint(Particle particle, StepVector step)
        : base(particle, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Particle
            .Nodes.Select(n =>
                Sin(n.Coordinates.Phi + (Angle.Half - n.RadiusNormalAngle.ToUpper))
                    * Step.NormalStress(n)
                + Sin(n.Coordinates.Phi + (Angle.Half - n.RadiusTangentAngle.ToUpper))
                    * Step.TangentialStress(n)
            )
            .Sum();

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        foreach (var n in Particle.Nodes)
        {
            yield return (
                Map.NormalStress(n),
                Sin(n.Coordinates.Phi + (Angle.Half - n.RadiusNormalAngle.ToUpper))
            );
            yield return (
                Map.TangentialStress(n),
                Sin(n.Coordinates.Phi + (Angle.Half - n.RadiusTangentAngle.ToUpper))
            );
        }
    }
}
