using RefraSin.Coordinates;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ParticleHorizontalForceBalanceConstraint : ParticleEquationBase
{
    /// <inheritdoc />
    public ParticleHorizontalForceBalanceConstraint(Particle particle, StepVector step)
        : base(particle, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Particle
            .Nodes.Select(n =>
                Cos(n.Coordinates.Phi + (Angle.Half - n.RadiusNormalAngle.ToUpper))
                    * Step.NormalStress(n)
                + Cos(n.Coordinates.Phi + (Angle.Half - n.RadiusTangentAngle.ToUpper))
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
                Cos(n.Coordinates.Phi + (Angle.Half - n.RadiusNormalAngle.ToUpper))
            );
            yield return (
                Map.TangentialStress(n),
                Cos(n.Coordinates.Phi + (Angle.Half - n.RadiusTangentAngle.ToUpper))
            );
        }
    }
}
