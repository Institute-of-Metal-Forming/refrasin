using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class RingContactConstraintY(
    SolutionState state,
    IGraphCycle<Particle, ParticleContact> ring,
    StepVector step
) : RingEquationBase(state, ring, step)
{
    /// <inheritdoc />
    public override double Value()
    {
        var primary = Ring.FirstPath.Sum(YShift);
        var secondary = Ring.SecondPath.Sum(YShift);

        return primary - secondary;
    }

    private double YShift(ParticleContact contact) =>
        Sin(contact.From.RotationAngle + contact.DirectionFrom) * Step.RadialDisplacement(contact)
        + contact.Distance
            * Cos(contact.From.RotationAngle + contact.DirectionFrom)
            * Step.AngleDisplacement(contact);

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        foreach (var contact in Ring.FirstPath)
        {
            yield return (Map.RadialDisplacement(contact), RadialDisplacementDerivative(contact));
            yield return (Map.AngleDisplacement(contact), AngleDisplacementDerivative(contact));
        }

        foreach (var contact in Ring.SecondPath)
        {
            yield return (Map.RadialDisplacement(contact), -RadialDisplacementDerivative(contact));
            yield return (Map.AngleDisplacement(contact), -AngleDisplacementDerivative(contact));
        }
    }

    public double RadialDisplacementDerivative(ParticleContact contact) =>
        Sin(contact.From.RotationAngle + contact.DirectionFrom);

    public double AngleDisplacementDerivative(ParticleContact contact) =>
        contact.Distance * Cos(contact.From.RotationAngle + contact.DirectionFrom);
}
