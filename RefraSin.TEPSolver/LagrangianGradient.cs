using RefraSin.Coordinates;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static MathNet.Numerics.Constants;

namespace RefraSin.TEPSolver;

internal static class LagrangianGradient
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
        foreach (var node in currentState.AllNodes.Values)
        {
            yield return StateVelocityDerivativeNormal(conditions, stepVector, node);
            yield return FluxDerivative(conditions, stepVector, node);
            yield return RequiredConstraint(conditions, stepVector, node);
        }

        // yield contact equations
        foreach (var contact in currentState.Contacts.Values)
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
                yield return stepVector[contactNode].LambdaContactDistance - stepVector[contactNode.ContactedNode].LambdaContactDistance;
                yield return stepVector[contactNode].LambdaContactDirection - stepVector[contactNode.ContactedNode].LambdaContactDirection;
            }

            // derivatives for aux variables
            yield return involvedNodes.Sum(n => stepVector[n].LambdaContactDistance);
            yield return involvedNodes.Sum(n => stepVector[n].LambdaContactDirection);
            yield return involvedNodes.Sum(n =>
            {
                var angleDifference = (n.ContactedNode.Coordinates.Phi - n.ContactedNode.ContactDirection).Reduce(Angle.ReductionDomain.WithNegative);
                return -n.ContactedNode.Coordinates.R * Sin(stepVector[contact].RotationDisplacement + angleDifference) *
                       stepVector[n].LambdaContactDistance
                     + n.ContactedNode.Coordinates.R / contact.Distance * Cos(stepVector[contact].RotationDisplacement + angleDifference) *
                       stepVector[n].LambdaContactDirection;
            });
        }

        yield return DissipationEquality(conditions, currentState, stepVector);
    }

    private static double StateVelocityDerivativeNormal(IProcessConditions conditions, StepVector stepVector, NodeBase node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Normal * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Normal * stepVector[node].LambdaVolume;

        double contactTerm = 0;

        if (node is ContactNodeBase contactNode)
        {
            contactTerm = contactNode.ContactDistanceGradient.Normal * stepVector[contactNode].LambdaContactDistance +
                          contactNode.ContactDirectionGradient.Normal * stepVector[contactNode].LambdaContactDirection;
        }

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double StateVelocityDerivativeTangential(IProcessConditions conditions, StepVector stepVector, ContactNodeBase node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Tangential * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Tangential * stepVector[node].LambdaVolume;
        var contactTerm = node.ContactDistanceGradient.Tangential * stepVector[node].LambdaContactDistance +
                          node.ContactDirectionGradient.Tangential * stepVector[node].LambdaContactDirection;

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double FluxDerivative(IProcessConditions conditions, StepVector stepVector, NodeBase node)
    {
        var dissipationTerm =
            2 * conditions.GasConstant * conditions.Temperature
          / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
          * node.SurfaceDistance.ToUpper * stepVector[node].FluxToUpper / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.Lambda1;
        var thisRequiredConstraintsTerm = stepVector[node].LambdaVolume;
        var upperRequiredConstraintsTerm = stepVector[node.Upper].LambdaVolume;

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private static double RequiredConstraint(IProcessConditions conditions, StepVector stepVector, NodeBase node)
    {
        var volumeTerm = node.VolumeGradient.Normal * stepVector[node].NormalDisplacement;
        var fluxTerm = stepVector[node].FluxToUpper - stepVector[node.Lower].FluxToUpper;

        return volumeTerm - fluxTerm;
    }

    private static double DissipationEquality(IProcessConditions conditions, SolutionState currentState, StepVector stepVector)
    {
        var dissipation = currentState.AllNodes.Values.Select(n =>
            -n.GibbsEnergyGradient.Normal * stepVector[n].NormalDisplacement
        ).Sum();

        var dissipationFunction = conditions.GasConstant * conditions.Temperature / 2
                                * currentState.AllNodes.Values.Select(n =>
                                      (
                                          n.SurfaceDistance.ToUpper * Math.Pow(stepVector[n].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToUpper
                                        + n.SurfaceDistance.ToLower * Math.Pow(stepVector[n.Lower].FluxToUpper, 2) /
                                          n.SurfaceDiffusionCoefficient.ToLower
                                      ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
                                  ).Sum();

        return dissipation - dissipationFunction;
    }

    private static (double distance, double direction) ContactConstraints(IProcessConditions conditions, StepVector stepVector,
        ParticleContact contact, ContactNodeBase node)
    {
        var normalShift = stepVector[node].NormalDisplacement + stepVector[node.ContactedNode].NormalDisplacement;
        var tangentialShift = stepVector[node].TangentialDisplacement + stepVector[node.ContactedNode].TangentialDisplacement;
        var rotationShift = 2 * node.ContactedNode.Coordinates.R * Sin(stepVector[contact].RotationDisplacement / 2);
        var rotationDirection = -(node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection) +
                                (Pi - stepVector[contact].RotationDisplacement) / 2;

        return (
            stepVector[contact].RadialDisplacement - node.ContactDistanceGradient.Normal * normalShift -
            node.ContactDistanceGradient.Tangential * tangentialShift + Cos(rotationDirection) * rotationShift,
            stepVector[contact].AngleDisplacement - node.ContactDirectionGradient.Normal * normalShift -
            node.ContactDirectionGradient.Tangential * tangentialShift - Sin(rotationDirection) / contact.Distance * rotationShift
        );
    }

    public static StepVector GuessSolution(SolutionState currentState) =>
        new(YieldInitialGuess(currentState).ToArray(),
            new StepVectorMap(currentState.ParticleContacts, currentState.AllNodes.Values));

    private static IEnumerable<double> YieldInitialGuess(SolutionState currentState) =>
        YieldGlobalUnknownsInitialGuess()
            .Concat(
                YieldContactUnknownsInitialGuess(currentState)
            )
            .Concat(
                YieldNodeUnknownsInitialGuess(currentState)
            ).Concat(
                YieldContactNodeUnknownsInitialGuess(currentState)
            );

    private static IEnumerable<double> YieldGlobalUnknownsInitialGuess()
    {
        yield return 1;
    }

    private static IEnumerable<double> YieldContactUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var _ in currentState.ParticleContacts)
        {
            yield return 0;
            yield return 0;
            yield return 0;
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var node in currentState.AllNodes.Values)
        {
            yield return node.GuessNormalDisplacement();
            yield return node.GuessFluxToUpper();
            yield return 1;
        }
    }

    private static IEnumerable<double> YieldContactNodeUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var _ in currentState.AllNodes.Values.OfType<ContactNodeBase>())
        {
            yield return 0;
            yield return 1;
            yield return 1;
        }
    }
}