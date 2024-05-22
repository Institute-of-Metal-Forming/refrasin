using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static RefraSin.TEPSolver.EquationSystem.Helper;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(ISinteringConditions conditions, SolutionState currentState) =>
        new(YieldInitialGuess(currentState).ToArray(), new StepVectorMap(currentState));

    private static IEnumerable<double> YieldInitialGuess(SolutionState currentState) =>
        Join(
            currentState.Particles.SelectMany(p => YieldParticleBlockGuesses(p)),
            YieldFunctionalBlockGuesses(currentState)
        );

    private static IEnumerable<double> YieldFunctionalBlockGuesses(SolutionState currentState) =>
        YieldContactUnknownsInitialGuess(currentState);

    private static IEnumerable<double> YieldParticleBlockGuesses(Particle particle) =>
        Join(YieldNodeUnknownsInitialGuess(particle.Nodes), YieldParticleUnknownsGuesses());

    private static IEnumerable<double> YieldParticleUnknownsGuesses()
    {
        yield return 1;
    }

    private static IEnumerable<double> YieldContactUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var contact in currentState.Contacts)
        {
            yield return 0;
            yield return 0;
            yield return 0;

            foreach (var node in contact.FromNodes)
            {
                yield return 0;
                yield return 0;

                if (node is NeckNode)
                    yield return 0;
            }

            foreach (var node in contact.ToNodes)
            {
                if (node is NeckNode)
                    yield return 0;
            }
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(IEnumerable<NodeBase> nodes)
    {
        foreach (var node in nodes)
        {
            yield return 0;
            yield return GuessFluxToUpper(node);
            yield return GuessNormalDisplacement(node);
        }
    }

    private static double GuessNormalDisplacement(NodeBase node)
    {
        var fluxBalance = GuessFluxToUpper(node) - GuessFluxToUpper(node.Lower);

        var displacement =
            2
          * fluxBalance
          / (
                (node.SurfaceDistance.ToUpper + node.SurfaceDistance.ToLower)
              * Math.Sin(node.SurfaceVectorAngle.Normal)
            );
        return displacement;
    }

    private static double GuessTangentialDisplacement(NodeBase node)
    {
        var fluxBalance = GuessFluxToUpper(node) - GuessFluxToUpper(node.Lower);

        var displacement =
            2
          * fluxBalance
          / (
                (node.SurfaceDistance.ToUpper + node.SurfaceDistance.ToLower)
              * Math.Sin(node.SurfaceVectorAngle.Tangential)
            );
        return displacement;
    }

    private static double GuessFluxToUpper(NodeBase node) =>
        -node.SurfaceDiffusionCoefficient.ToUpper * (GuessVacancyConcentration(node) - GuessVacancyConcentration(node.Upper))
      / Math.Pow(node.SurfaceDistance.ToUpper, 2);

    private static double GuessVacancyConcentration(NodeBase node) =>
        (node is not NeckNode ? node.GibbsEnergyGradient.Normal : node.GibbsEnergyGradient.Normal)
      / node.Particle.VacancyVolumeEnergy;
}