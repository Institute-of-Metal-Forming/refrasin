using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using GrainBoundaryNode = RefraSin.TEPSolver.ParticleModel.GrainBoundaryNode;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(ISinteringConditions conditions, SolutionState currentState)
    {
        var map = new StepVectorMap(currentState);
        var vector = new StepVector(new double[map.TotalLength], map);
        FillStepVector(vector, currentState);
        return vector;
    }

    private static void FillStepVector(StepVector stepVector, SolutionState currentState)
    {
        stepVector.LambdaDissipation(1);

        foreach (var particle in currentState.Particles)
        {
            foreach (var node in particle.Nodes)
            {
                if (node.Type is NodeType.Surface)
                {
                    stepVector.NormalDisplacement(node, GuessNormalDisplacement(node));
                }

                stepVector.FluxToUpper(node, GuessFluxToUpper(node));
                stepVector.LambdaVolume(node, 1);
            }
        }

        foreach (var contact in currentState.ParticleContacts)
        {
            var averageNormalDisplacement =
                contact.FromNodes.OfType<GrainBoundaryNode>().Average(GuessNormalDisplacement)
                + contact.ToNodes.OfType<GrainBoundaryNode>().Average(GuessNormalDisplacement);

            foreach (var node in contact.FromNodes)
            {
                stepVector.LambdaContactX(node, 1);
                stepVector.LambdaContactY(node, 1);

                stepVector.NormalDisplacement(node, averageNormalDisplacement);
                stepVector.NormalDisplacement(node.ContactedNode, averageNormalDisplacement);

                if (node.Type is NodeType.Neck)
                {
                    stepVector.TangentialDisplacement(node, GuessTangentialDisplacement(node));
                    stepVector.TangentialDisplacement(
                        node.ContactedNode,
                        GuessTangentialDisplacement(node.ContactedNode)
                    );
                }
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
        -node.InterfaceDiffusionCoefficient.ToUpper
        * (GuessVacancyConcentration(node) - GuessVacancyConcentration(node.Upper))
        / Pow(node.SurfaceDistance.ToUpper, 2);

    private static double GuessVacancyConcentration(NodeBase node) =>
        (
            node is not NeckNode
                ? node.GibbsEnergyGradient.Normal
                : -Abs(node.GibbsEnergyGradient.Tangential)
        ) / node.Particle.VacancyVolumeEnergy;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
