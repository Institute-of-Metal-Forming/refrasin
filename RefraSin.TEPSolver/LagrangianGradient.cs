using MathNet.Numerics.RootFinding;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

internal static class LagrangianGradient
{
    public static StepVector EvaluateAt(ISolverSession solverSession, StepVector stepVector)
    {
        var evaluation = YieldEquations(solverSession, stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return new StepVector(evaluation, stepVector.StepVectorMap);
    }

    private static IEnumerable<double> YieldEquations(ISolverSession solverSession, StepVector stepVector)
    {
        // fix root particle to origin
        var root = stepVector.StepVectorMap.Particles[0];
        yield return stepVector[root].RadialDisplacement;
        yield return stepVector[root].AngleDisplacement;
        yield return stepVector[root].RotationDisplacement;

        // yield particle displacement equations
        foreach (var particle in stepVector.StepVectorMap.Particles.Skip(1))
        {
            yield return stepVector[particle].RadialDisplacement;
            yield return stepVector[particle].AngleDisplacement;
            yield return stepVector[particle].RotationDisplacement;
        }

        // yield node equations
        foreach (var node in stepVector.StepVectorMap.Nodes)
        {
            yield return StateVelocityDerivative(solverSession, stepVector, node);
            yield return FluxDerivative(solverSession, stepVector, node);
            yield return RequiredConstraint(solverSession, stepVector, node);
        }

        yield return DissipationEquality(solverSession, stepVector);
    }

    private static double StateVelocityDerivative(ISolverSession solverSession, StepVector stepVector, NodeBase node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Normal * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Normal * stepVector[node].Lambda2;

        return gibbsTerm + requiredConstraintsTerm;
    }

    private static double FluxDerivative(ISolverSession solverSession, StepVector stepVector, NodeBase node)
    {
        var dissipationTerm =
            2 * solverSession.GasConstant * solverSession.Temperature
          / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
          * node.SurfaceDistance.ToUpper * stepVector[node].FluxToUpper / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.Lambda1;
        var thisRequiredConstraintsTerm = stepVector[node].Lambda2;
        var upperRequiredConstraintsTerm = stepVector[node.Upper].Lambda2;

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private static double RequiredConstraint(ISolverSession solverSession, StepVector stepVector, NodeBase node)
    {
        var volumeTerm = node.VolumeGradient.Normal * stepVector[node].NormalDisplacement;
        var fluxTerm = stepVector[node].FluxToUpper - stepVector[node.Lower].FluxToUpper;

        return volumeTerm - fluxTerm;
    }

    private static double DissipationEquality(ISolverSession solverSession, StepVector stepVector)
    {
        var dissipation = stepVector.StepVectorMap.Nodes.Select(n =>
            -n.GibbsEnergyGradient.Normal * stepVector[n].NormalDisplacement
        ).Sum();

        var dissipationFunction = solverSession.GasConstant * solverSession.Temperature / 2
                                * stepVector.StepVectorMap.Nodes.Select(n =>
                                      (
                                          n.SurfaceDistance.ToUpper * Math.Pow(stepVector[n].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToUpper
                                        + n.SurfaceDistance.ToLower * Math.Pow(stepVector[n.Lower].FluxToUpper, 2) /
                                          n.SurfaceDiffusionCoefficient.ToLower
                                      ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
                                  ).Sum();

        return dissipation - dissipationFunction;
    }

    public static StepVector GuessSolution(ISolverSession solverSession) =>
        new(YieldInitialGuess(solverSession).ToArray(),
            new StepVectorMap(solverSession.CurrentState.Particles, solverSession.CurrentState.AllNodes.Values));

    private static IEnumerable<double> YieldInitialGuess(ISolverSession solverSession) =>
        YieldGlobalUnknownsInitialGuess()
            .Concat(YieldParticleUnknownsInitialGuess(solverSession)
            )
            .Concat(
                YieldNodeUnknownsInitialGuess(solverSession)
            );

    private static IEnumerable<double> YieldGlobalUnknownsInitialGuess()
    {
        yield return 1;
    }

    private static IEnumerable<double> YieldParticleUnknownsInitialGuess(ISolverSession solverSession)
    {
        foreach (var particle in solverSession.CurrentState.Particles)
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
}