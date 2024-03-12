using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(IProcessConditions conditions, SolutionState currentState) =>
        new(YieldInitialGuess(conditions, currentState).ToArray(), new StepVectorMap(currentState));

    private static IEnumerable<double> YieldInitialGuess(
        IProcessConditions conditions,
        SolutionState currentState
    ) =>
        YieldNodeUnknownsInitialGuess(conditions, currentState)
            .Concat(YieldContactUnknownsInitialGuess(currentState))
            .Concat(YieldGlobalUnknownsInitialGuess());

    private static IEnumerable<double> YieldGlobalUnknownsInitialGuess()
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
                if (node is NeckNode)
                    yield return 0;
                yield return 1;
                yield return 1;
            }

            foreach (var _ in contact.ToNodes.OfType<NeckNode>())
            {
                yield return 0;
            }
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(
        IProcessConditions conditions,
        SolutionState currentState
    )
    {
        foreach (var node in currentState.Nodes)
        {
            yield return GuessNormalDisplacement(conditions, node);
            yield return GuessFluxToUpper(conditions, node);
            yield return 1;
        }
    }

    private static double GuessNormalDisplacement(IProcessConditions conditions, NodeBase node)
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

    private static double GuessFluxToUpper(IProcessConditions conditions, NodeBase node)
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
