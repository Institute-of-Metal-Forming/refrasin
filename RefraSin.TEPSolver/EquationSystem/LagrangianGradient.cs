using RefraSin.Coordinates;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static MathNet.Numerics.Constants;

namespace RefraSin.TEPSolver.EquationSystem;

public static class LagrangianGradient
{
    public static StepVector EvaluateAt(IProcessConditions conditions, SolutionState currentState, StepVector stepVector)
    {
        var evaluation = YieldEquations(conditions, currentState, stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return new StepVector(evaluation, stepVector.StepVectorMap);
    }

    public static IEnumerable<double> YieldEquations(IProcessConditions conditions, SolutionState currentState, StepVector stepVector)
    {
        // yield node equations
        foreach (var node in currentState.Nodes)
        {
            yield return StateVelocityDerivativeNormal(conditions, stepVector, node);
            yield return FluxDerivative(conditions, stepVector, node);
            yield return RequiredConstraint(conditions, stepVector, node);
        }

        // yield contact equations
        foreach (var contact in currentState.Contacts)
        {
            var involvedNodes = contact.From.Nodes.OfType<ContactNodeBase>().Where(n => n.ContactedParticleId == contact.To.Id).ToArray();

            foreach (var contactNode in involvedNodes)
            {
                yield return StateVelocityDerivativeTangential(conditions, stepVector, contactNode);
                yield return StateVelocityDerivativeTangential(conditions, stepVector, contactNode.ContactedNode);

                var constraints = ContactConstraints(conditions, stepVector, contact, contactNode);
                yield return constraints.distance;
                yield return constraints.direction;

                // lambdas of contact must be equal for both connected nodes
                yield return stepVector.LambdaContactDistance(contactNode) - stepVector.LambdaContactDistance(contactNode.ContactedNode);
                yield return stepVector.LambdaContactDirection(contactNode) - stepVector.LambdaContactDirection(contactNode.ContactedNode);
            }

            // derivatives for aux variables
            yield return involvedNodes.Sum(stepVector.LambdaContactDistance);
            yield return involvedNodes.Sum(stepVector.LambdaContactDirection);
            yield return involvedNodes.Sum(n =>
            {
                var angleDifference = (n.ContactedNode.Coordinates.Phi - n.ContactedNode.ContactDirection).Reduce(Angle.ReductionDomain.WithNegative);
                return -n.ContactedNode.Coordinates.R * Sin(stepVector.RotationDisplacement(contact) + angleDifference) *
                       stepVector.LambdaContactDistance(n)
                     + n.ContactedNode.Coordinates.R / contact.Distance * Cos(stepVector.RotationDisplacement(contact) + angleDifference) *
                       stepVector.LambdaContactDirection(n);
            });
        }

        yield return DissipationEquality(conditions, currentState, stepVector);
    }

    private static double StateVelocityDerivativeNormal(IProcessConditions conditions, StepVector stepVector, NodeBase node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Normal * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Normal * stepVector.LambdaVolume(node);

        double contactTerm = 0;

        if (node is ContactNodeBase contactNode)
        {
            contactTerm = contactNode.ContactDistanceGradient.Normal * stepVector.LambdaContactDistance(contactNode) +
                          contactNode.ContactDirectionGradient.Normal * stepVector.LambdaContactDirection(contactNode);
        }

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double StateVelocityDerivativeTangential(IProcessConditions conditions, StepVector stepVector, ContactNodeBase node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Tangential * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Tangential * stepVector.LambdaVolume(node);
        var contactTerm = node.ContactDistanceGradient.Tangential * stepVector.LambdaContactDistance(node) +
                          node.ContactDirectionGradient.Tangential * stepVector.LambdaContactDirection(node);

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double FluxDerivative(IProcessConditions conditions, StepVector stepVector, NodeBase node)
    {
        var dissipationTerm =
            2 * conditions.GasConstant * conditions.Temperature
          / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
          * node.SurfaceDistance.ToUpper * stepVector.FluxToUpper(node) / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.Lambda1;
        var thisRequiredConstraintsTerm = stepVector.LambdaVolume(node);
        var upperRequiredConstraintsTerm = stepVector.LambdaVolume(node.Upper);

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private static double RequiredConstraint(IProcessConditions conditions, StepVector stepVector, NodeBase node)
    {
        var normalVolumeTerm = node.VolumeGradient.Normal * stepVector.NormalDisplacement(node);
        var tangentialVolumeTerm = 0.0;

        if (node is ContactNodeBase contactNode)
        {
            tangentialVolumeTerm = node.VolumeGradient.Tangential * stepVector.TangentialDisplacement(contactNode);
        }

        var fluxTerm = stepVector.FluxToUpper(node) - stepVector.FluxToUpper(node.Lower);

        return normalVolumeTerm + tangentialVolumeTerm - fluxTerm;
    }

    private static double DissipationEquality(IProcessConditions conditions, SolutionState currentState, StepVector stepVector)
    {
        var dissipationNormal = currentState.Nodes.Select(n =>
            -n.GibbsEnergyGradient.Normal * stepVector.NormalDisplacement(n)
        ).Sum();
        
        var dissipationTangential = currentState.Nodes.OfType<ContactNodeBase>().Select(n =>
            -n.GibbsEnergyGradient.Tangential * stepVector.TangentialDisplacement(n)
        ).Sum();

        var dissipationFunction = conditions.GasConstant * conditions.Temperature / 2
                                * currentState.Nodes.Select(n =>
                                      (
                                          n.SurfaceDistance.ToUpper * Math.Pow(stepVector.FluxToUpper(n), 2) / n.SurfaceDiffusionCoefficient.ToUpper
                                        + n.SurfaceDistance.ToLower * Math.Pow(stepVector.FluxToUpper(n.Lower), 2) /
                                          n.SurfaceDiffusionCoefficient.ToLower
                                      ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
                                  ).Sum();

        return dissipationNormal + dissipationTangential - dissipationFunction;
    }

    private static (double distance, double direction) ContactConstraints(IProcessConditions conditions, StepVector stepVector,
        ParticleContact contact, ContactNodeBase node)
    {
        var normalShift = stepVector.NormalDisplacement(node) + stepVector.NormalDisplacement(node.ContactedNode);
        var tangentialShift = stepVector.TangentialDisplacement(node) + stepVector.TangentialDisplacement(node.ContactedNode);
        var rotationShift = 2 * node.ContactedNode.Coordinates.R * Sin(stepVector.RotationDisplacement(contact) / 2);
        var rotationDirection = -(node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection) +
                                (Pi - stepVector.RotationDisplacement(contact)) / 2;

        return (
            stepVector.RadialDisplacement(contact) - node.ContactDistanceGradient.Normal * normalShift -
            node.ContactDistanceGradient.Tangential * tangentialShift + Cos(rotationDirection) * rotationShift,
            stepVector.AngleDisplacement(contact) - node.ContactDirectionGradient.Normal * normalShift -
            node.ContactDirectionGradient.Tangential * tangentialShift - Sin(rotationDirection) / contact.Distance * rotationShift
        );
    }

    public static StepVector GuessSolution(SolutionState currentState) =>
        new(YieldInitialGuess(currentState).ToArray(),
            new StepVectorMap(currentState));

    private static IEnumerable<double> YieldInitialGuess(SolutionState currentState) =>
        YieldNodeUnknownsInitialGuess(currentState)
            .Concat(
                YieldContactNodeUnknownsInitialGuess(currentState)
            )
            .Concat(
                YieldContactUnknownsInitialGuess(currentState)
            ).Concat(
                YieldGlobalUnknownsInitialGuess()
            );

    private static IEnumerable<double> YieldGlobalUnknownsInitialGuess()
    {
        yield return 1;
    }

    private static IEnumerable<double> YieldContactUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var _ in currentState.Contacts)
        {
            yield return 0;
            yield return 0;
            yield return 0;
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var node in currentState.Nodes)
        {
            yield return node.GuessNormalDisplacement();
            yield return node.GuessFluxToUpper();
            yield return 1;
        }
    }

    private static IEnumerable<double> YieldContactNodeUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var _ in currentState.Nodes.OfType<ContactNodeBase>())
        {
            yield return 0;
            yield return 1;
            yield return 1;
        }
    }
}
