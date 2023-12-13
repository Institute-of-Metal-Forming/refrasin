using MathNet.Numerics.RootFinding;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static MathNet.Numerics.Constants;

namespace RefraSin.TEPSolver;

internal static class LagrangianGradient
{
    public static StepVector EvaluateAt(ISolverSession solverSession, SolutionState currentState, StepVector stepVector)
    {
        var evaluation = YieldEquations(solverSession, currentState, stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return new StepVector(evaluation, stepVector.StepVectorMap);
    }

    private static IEnumerable<double> YieldEquations(ISolverSession solverSession, SolutionState currentState, StepVector stepVector)
    {
        // yield contact equations
        foreach (var contact in currentState.Contacts.Values)
        {
            var involvedNodes = contact.From.Nodes.OfType<ContactNodeBase>().Where(n => n.ContactedParticleId == contact.To.Id).ToArray();

            foreach (var contactNode in involvedNodes)
            {
                var constraints = ContactConstraints(solverSession, stepVector, contact, contactNode);
                yield return constraints.distance;
                yield return constraints.direction;
                yield return stepVector[contactNode].LambdaContactDistance - stepVector[contactNode.ContactedNode].LambdaContactDistance;
                yield return stepVector[contactNode].LambdaContactDirection - stepVector[contactNode.ContactedNode].LambdaContactDirection;
            }

            yield return involvedNodes.Sum(n => stepVector[n].LambdaContactDistance);
            yield return involvedNodes.Sum(n => stepVector[n].LambdaContactDirection);
            // TODO: yield derivative for rotation
        }

        // yield node equations
        foreach (var node in currentState.AllNodes.Values)
        {
            yield return StateVelocityDerivativeNormal(solverSession, stepVector, node);

            if (node is ContactNodeBase contactNode)
                yield return StateVelocityDerivativeTangential(solverSession, stepVector, contactNode);

            yield return FluxDerivative(solverSession, stepVector, node);
            yield return RequiredConstraint(solverSession, stepVector, node);
        }

        yield return DissipationEquality(solverSession, currentState, stepVector);
    }

    private static double StateVelocityDerivativeNormal(ISolverSession solverSession, StepVector stepVector, NodeBase node)
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

    private static double StateVelocityDerivativeTangential(ISolverSession solverSession, StepVector stepVector, ContactNodeBase node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Tangential * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Tangential * stepVector[node].LambdaVolume;
        var contactTerm = node.ContactDistanceGradient.Tangential * stepVector[node].LambdaContactDistance +
                          node.ContactDirectionGradient.Tangential * stepVector[node].LambdaContactDirection;

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double FluxDerivative(ISolverSession solverSession, StepVector stepVector, NodeBase node)
    {
        var dissipationTerm =
            2 * solverSession.GasConstant * solverSession.Temperature
          / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
          * node.SurfaceDistance.ToUpper * stepVector[node].FluxToUpper / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.Lambda1;
        var thisRequiredConstraintsTerm = stepVector[node].LambdaVolume;
        var upperRequiredConstraintsTerm = stepVector[node.Upper].LambdaVolume;

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private static double RequiredConstraint(ISolverSession solverSession, StepVector stepVector, NodeBase node)
    {
        var volumeTerm = node.VolumeGradient.Normal * stepVector[node].NormalDisplacement;
        var fluxTerm = stepVector[node].FluxToUpper - stepVector[node.Lower].FluxToUpper;

        return volumeTerm - fluxTerm;
    }

    private static double DissipationEquality(ISolverSession solverSession, SolutionState currentState, StepVector stepVector)
    {
        var dissipation = currentState.AllNodes.Values.Select(n =>
            -n.GibbsEnergyGradient.Normal * stepVector[n].NormalDisplacement
        ).Sum();

        var dissipationFunction = solverSession.GasConstant * solverSession.Temperature / 2
                                * currentState.AllNodes.Values.Select(n =>
                                      (
                                          n.SurfaceDistance.ToUpper * Math.Pow(stepVector[n].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToUpper
                                        + n.SurfaceDistance.ToLower * Math.Pow(stepVector[n.Lower].FluxToUpper, 2) /
                                          n.SurfaceDiffusionCoefficient.ToLower
                                      ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
                                  ).Sum();

        return dissipation - dissipationFunction;
    }

    private static (double distance, double direction) ContactConstraints(ISolverSession solverSession, StepVector stepVector,
        ParticleContact contact, ContactNodeBase node)
    {
        var normalShift = stepVector[node].NormalDisplacement + stepVector[node.ContactedNode].NormalDisplacement;
        var tangentialShift = stepVector[node].TangentialDisplacement + stepVector[node.ContactedNode].TangentialDisplacement;
        var rotationShift = node.Coordinates.R / Sin((Pi - stepVector[contact].RotationDisplacement) / 2) *
                            Sin(stepVector[contact].RotationDisplacement);
        var rotationDirection = -(node.Coordinates.Phi - node.ContactDirection) + (PiOver2 - stepVector[contact].RotationDisplacement) / 2;

        return (
            stepVector[contact].RadialDisplacement - node.ContactDistanceGradient.Normal * normalShift -
            node.ContactDistanceGradient.Tangential * tangentialShift + Cos(rotationDirection) * rotationShift,
            stepVector[contact].AngleDisplacement - node.ContactDirectionGradient.Normal * normalShift -
            node.ContactDirectionGradient.Tangential * tangentialShift - Sin(rotationDirection) / contact.Distance * rotationShift
        );
    }

    public static StepVector GuessSolution(ISolverSession solverSession) =>
        new(YieldInitialGuess(solverSession).ToArray(),
            new StepVectorMap(solverSession.CurrentState.Contacts.Values, solverSession.CurrentState.AllNodes.Values));

    private static IEnumerable<double> YieldInitialGuess(ISolverSession solverSession) =>
        YieldGlobalUnknownsInitialGuess()
            .Concat(
                YieldContactUnknownsInitialGuess(solverSession)
            )
            .Concat(
                YieldNodeUnknownsInitialGuess(solverSession)
            ).Concat(
                YieldContactNodeUnknownsInitialGuess(solverSession)
            );

    private static IEnumerable<double> YieldGlobalUnknownsInitialGuess()
    {
        yield return 1;
    }

    private static IEnumerable<double> YieldContactUnknownsInitialGuess(ISolverSession solverSession)
    {
        foreach (var _ in solverSession.CurrentState.Contacts.Values)
        {
            yield return 0;
            yield return 0;
            yield return 0;
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(ISolverSession solverSession)
    {
        foreach (var node in solverSession.CurrentState.AllNodes.Values)
        {
            yield return node.GuessNormalDisplacement();
            yield return node.GuessFluxToUpper();
            yield return 1;
        }
    }

    private static IEnumerable<double> YieldContactNodeUnknownsInitialGuess(ISolverSession solverSession)
    {
        foreach (var _ in solverSession.CurrentState.AllNodes.Values.OfType<ContactNodeBase>())
        {
            yield return 0;
            yield return 1;
            yield return 1;
        }
    }
}