using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ContactDirectionDerivative : ContactEquationBase
{
    /// <inheritdoc />
    public ContactDirectionDerivative(SolutionState state, ParticleContact contact, StepVector step)
        : base(state, contact, step)
    {
        _ringsWithThisInPrimary = State
            .ParticleCycles.Where(r => r.FirstPath.Contains(Contact))
            .ToArray();
        _ringsWithThisInSecondary = State
            .ParticleCycles.Where(r => r.SecondPath.Contains(Contact))
            .ToArray();
        _xPreFactor =
            -(contact.Distance + Step.RadialDisplacement(contact))
            * Sin(
                Contact.From.RotationAngle + Contact.DirectionFrom + Step.AngleDisplacement(Contact)
            );
        _yPreFactor =
            (contact.Distance + Step.RadialDisplacement(contact))
            * Cos(
                Contact.From.RotationAngle + Contact.DirectionFrom + Step.AngleDisplacement(Contact)
            );
    }

    /// <inheritdoc />
    public override double Value()
    {
        var contactConstraintsTerm = Contact.FromNodes.Sum(Step.LambdaContactDirection);

        var ringsWithThisContactInPrimaryTerm = _ringsWithThisInPrimary.Select(CycleTerm).Sum();
        var ringsWithThisContactInSecondaryTerm = _ringsWithThisInSecondary.Select(CycleTerm).Sum();

        return contactConstraintsTerm
            + ringsWithThisContactInPrimaryTerm
            - ringsWithThisContactInSecondaryTerm;

        double CycleTerm(IGraphCycle<Particle, ParticleContact> cycle) =>
            _xPreFactor * Step.LambdaCycleX(cycle) + _yPreFactor * Step.LambdaCycleY(cycle);
    }

    private readonly IReadOnlyList<IGraphCycle<Particle, ParticleContact>> _ringsWithThisInPrimary;
    private readonly IReadOnlyList<
        IGraphCycle<Particle, ParticleContact>
    > _ringsWithThisInSecondary;

    private readonly double _xPreFactor;
    private readonly double _yPreFactor;

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        foreach (var fromNode in Contact.FromNodes)
        {
            yield return (Map.LambdaContactDirection(fromNode), 1.0);
        }

        yield return (
            Map.RadialDisplacement(Contact),
            -Sin(
                Contact.From.RotationAngle + Contact.DirectionFrom + Step.AngleDisplacement(Contact)
            )
                * (
                    _ringsWithThisInPrimary.Sum(Step.LambdaCycleX)
                    - _ringsWithThisInSecondary.Sum(Step.LambdaCycleX)
                )
                + Cos(
                    Contact.From.RotationAngle
                        + Contact.DirectionFrom
                        + Step.AngleDisplacement(Contact)
                )
                    * (
                        _ringsWithThisInPrimary.Sum(Step.LambdaCycleY)
                        - _ringsWithThisInSecondary.Sum(Step.LambdaCycleY)
                    )
        );

        yield return (
            Map.AngleDisplacement(Contact),
            -(Contact.Distance + Step.RadialDisplacement(Contact))
                * Cos(
                    Contact.From.RotationAngle
                        + Contact.DirectionFrom
                        + Step.AngleDisplacement(Contact)
                )
                * (
                    _ringsWithThisInPrimary.Sum(Step.LambdaCycleX)
                    - _ringsWithThisInSecondary.Sum(Step.LambdaCycleX)
                )
                - (Contact.Distance + Step.RadialDisplacement(Contact))
                    * Sin(
                        Contact.From.RotationAngle
                            + Contact.DirectionFrom
                            + Step.AngleDisplacement(Contact)
                    )
                    * (
                        _ringsWithThisInPrimary.Sum(Step.LambdaCycleY)
                        - _ringsWithThisInSecondary.Sum(Step.LambdaCycleY)
                    )
        );

        foreach (var cycle in _ringsWithThisInPrimary)
        {
            yield return (Map.LambdaCycleX(cycle), _xPreFactor);
            yield return (Map.LambdaCycleY(cycle), _yPreFactor);
        }

        foreach (var cycle in _ringsWithThisInSecondary)
        {
            yield return (Map.LambdaCycleX(cycle), -_xPreFactor);
            yield return (Map.LambdaCycleY(cycle), -_yPreFactor);
        }
    }
}
