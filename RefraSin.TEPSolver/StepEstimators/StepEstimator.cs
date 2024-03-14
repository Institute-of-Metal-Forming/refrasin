using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static RefraSin.TEPSolver.EquationSystem.Helper;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(ISinteringConditions conditions, SolutionState currentState) =>
        new(YieldInitialGuess(conditions, currentState).ToArray(), new StepVectorMap(currentState));

    private static IEnumerable<double> YieldInitialGuess(
        ISinteringConditions conditions,
        SolutionState currentState
    ) => Join(
        currentState.Particles.SelectMany(p => YieldParticleBlockGuesses(conditions, p)),
        YieldFunctionalBlockGuesses(conditions, currentState)
    );

    private static IEnumerable<double> YieldFunctionalBlockGuesses(IProcessConditions conditions, SolutionState currentState) =>
        YieldContactUnknownsInitialGuess(currentState);

    private static IEnumerable<double> YieldParticleBlockGuesses(IProcessConditions conditions, Particle particle) => Join(
        YieldNodeUnknownsInitialGuess(conditions, particle.Nodes),
        YieldParticleUnknownsGuesses()
    );

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
                yield return 1;
                yield return 1;
            }
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(
        ISinteringConditions conditions,
        IEnumerable<NodeBase> nodes
    )
    {
        foreach (var node in nodes)
        {
            yield return 1;
            yield return GuessFluxToUpper(conditions, node);
            yield return GuessNormalDisplacement(conditions, node);
            
            if (node is NeckNode)
                yield return 0;
        }
    }

    private static double GuessNormalDisplacement(ISinteringConditions conditions, NodeBase node)
    {
        var fluxBalance =
            GuessFluxToUpper(conditions, node) - GuessFluxToUpper(conditions, node.Lower);

        var displacement =
            2
          * fluxBalance
          / (
                (node.SurfaceDistance.ToUpper + node.SurfaceDistance.ToLower)
              * Math.Sin(node.SurfaceVectorAngle.Normal)
            );
        return displacement;
    }

    private static double GuessFluxToUpper(ISinteringConditions conditions, NodeBase node)
    {
        var vacancyConcentrationGradient =
            -node.Particle.Material.EquilibriumVacancyConcentration
          / (conditions.GasConstant * conditions.Temperature)
          * (node.Upper.GibbsEnergyGradient.Normal - node.GibbsEnergyGradient.Normal)
          * node.Particle.Material.MolarVolume
          / Math.Pow(node.SurfaceDistance.ToUpper, 2);
        return -node.SurfaceDiffusionCoefficient.ToUpper * vacancyConcentrationGradient;
    }
}
