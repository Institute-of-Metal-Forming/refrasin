using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static RefraSin.TEPSolver.EquationSystem.Helper;
using GrainBoundaryNode = RefraSin.TEPSolver.ParticleModel.GrainBoundaryNode;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(ISinteringConditions conditions, SolutionState currentState) =>
        new(YieldInitialGuess(currentState).ToArray(), new StepVectorMap(currentState));

    private static IEnumerable<double> YieldInitialGuess(SolutionState currentState) =>
        Join(
            currentState.Particles.SelectMany(p => YieldParticleBlockGuesses(p)),
            YieldFunctionalBlockGuesses(currentState)
        ).Append(1);

    private static IEnumerable<double> YieldFunctionalBlockGuesses(SolutionState currentState) =>
        YieldContactUnknownsInitialGuess(currentState);

    private static IEnumerable<double> YieldParticleBlockGuesses(Particle particle) =>
        YieldNodeUnknownsInitialGuess(particle.Nodes);

    private static IEnumerable<double> YieldContactUnknownsInitialGuess(SolutionState currentState)
    {
        foreach (var contact in currentState.ParticleContacts)
        {
            var averageNormalDisplacement = contact.FromNodes.OfType<GrainBoundaryNode>().Average(GuessNormalDisplacement) +
                                            contact.ToNodes.OfType<GrainBoundaryNode>().Average(GuessNormalDisplacement);
            yield return averageNormalDisplacement;
            yield return 0;

            foreach (var node in contact.FromNodes)
            {
                yield return 0;
                yield return 0;

                yield return 0;
                yield return GuessFluxToUpper(node);
                yield return averageNormalDisplacement;

                yield return 0;
                yield return GuessFluxToUpper(node.ContactedNode);
                yield return averageNormalDisplacement;

                if (node is NeckNode)
                {
                    yield return GuessTangentialDisplacement(node);
                    yield return GuessTangentialDisplacement(node.ContactedNode);
                }
            }
        }
    }

    private static IEnumerable<double> YieldNodeUnknownsInitialGuess(IEnumerable<NodeBase> nodes)
    {
        foreach (var node in nodes)
        {
            if (node is not IContactNode)
            {
                yield return 0;
                yield return GuessFluxToUpper(node);
                yield return GuessNormalDisplacement(node);
            }
        }
    }

    private static double GuessNormalDisplacement(NodeBase node)
    {
        var fluxBalance = GuessFluxToUpper(node) - GuessFluxToUpper(node.Lower);

        var displacement = fluxBalance / node.VolumeGradient.Normal;
        return displacement;
    }

    private static double GuessTangentialDisplacement(NodeBase node)
    {
        var fluxBalance = GuessFluxToUpper(node) - GuessFluxToUpper(node.Lower);

        var displacement = fluxBalance / node.VolumeGradient.Tangential;
        return displacement;
    }

    private static double GuessFluxToUpper(NodeBase node) =>
        -node.SurfaceDiffusionCoefficient.ToUpper * (GuessVacancyConcentration(node) - GuessVacancyConcentration(node.Upper))
      / Pow(node.SurfaceDistance.ToUpper, 2);

    private static double GuessVacancyConcentration(NodeBase node) =>
        (node is not NeckNode ? node.GibbsEnergyGradient.Normal : -Abs(node.GibbsEnergyGradient.Tangential))
      / node.Particle.VacancyVolumeEnergy;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}