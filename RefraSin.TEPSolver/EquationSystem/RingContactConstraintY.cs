using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class RingContactConstraintY(IGraphCycle<Particle, ParticleContact> ring, StepVector step)
    : RingEquationBase(ring, step)
{
    /// <inheritdoc />
    public override double Value()
    {
        var primary = Ring.FirstPath.Sum(YShift);
        var secondary = Ring.SecondPath.Sum(YShift);

        return primary - secondary;
    }

    private double YShift(ParticleContact contact)
    {
        var oldX = contact.Distance * Sin(contact.From.RotationAngle + contact.DirectionFrom);
        var newX =
            (contact.Distance + Step.RadialDisplacement(contact))
            * Sin(
                contact.From.RotationAngle + contact.DirectionFrom + Step.AngleDisplacement(contact)
            );

        return newX - oldX;
    }

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

    private double RadialDisplacementDerivative(ParticleContact contact) =>
        Sin(contact.From.RotationAngle + contact.DirectionFrom + Step.AngleDisplacement(contact));

    private double AngleDisplacementDerivative(ParticleContact contact) =>
        (contact.Distance + Step.RadialDisplacement(contact))
        * Cos(contact.From.RotationAngle + contact.DirectionFrom + Step.AngleDisplacement(contact));
}
