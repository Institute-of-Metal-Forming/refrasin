using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ParticleTorqueBalanceConstraint : ParticleEquationBase
{
    /// <inheritdoc />
    public ParticleTorqueBalanceConstraint(Particle particle, StepVector step)
        : base(particle, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Particle
            .Nodes.Select(n =>
                Step.NormalStress(n) * Sin(n.RadiusNormalAngle.ToUpper) * n.Coordinates.R
                + Step.TangentialStress(n) * Sin(n.RadiusTangentAngle.ToUpper) * n.Coordinates.R
            )
            .Sum();

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        foreach (var n in Particle.Nodes)
        {
            yield return (Map.NormalStress(n), Sin(n.RadiusNormalAngle.ToUpper) * n.Coordinates.R);
            yield return (
                Map.TangentialStress(n),
                Sin(n.RadiusTangentAngle.ToUpper) * n.Coordinates.R
            );
        }
    }
}
