using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using GrainBoundaryNode = RefraSin.TEPSolver.ParticleModel.GrainBoundaryNode;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(EquationSystem equationSystem)
    {
        var map = new StepVectorMap(equationSystem);
        var vector = new StepVector(new double[map.TotalLength], map);
        FillStepVector(vector, equationSystem.State);
        return vector;
    }

    private static void FillStepVector(StepVector stepVector, SolutionState currentState)
    {
        stepVector.SetConstraintLambdaValue<DissipationEqualityConstraint>(1);

        foreach (var particle in currentState.Particles)
        {
            foreach (var node in particle.Nodes)
            {
                if (node.Type is NodeType.Surface)
                {
                    stepVector.SetQuantityValue<NormalDisplacement>(
                        node,
                        GuessNormalDisplacement(node)
                    );
                }

                stepVector.SetQuantityValue<FluxToUpper>(node, GuessFluxToUpper(node));
                stepVector.SetConstraintLambdaValue<VolumeBalanceConstraint>(node, 1);
            }
        }

        foreach (var contact in currentState.ParticleContacts)
        {
            var averageNormalDisplacement =
                contact
                    .FromNodes<Particle, NodeBase>()
                    .OfType<GrainBoundaryNode>()
                    .Average(GuessNormalDisplacement)
                + contact
                    .ToNodes<Particle, NodeBase>()
                    .OfType<GrainBoundaryNode>()
                    .Average(GuessNormalDisplacement);

            foreach (var node in contact.FromNodes<Particle, NodeBase>().OfType<ContactNodeBase>())
            {
                stepVector.SetConstraintLambdaValue<ContactConstraintX>(node, 1);
                stepVector.SetConstraintLambdaValue<ContactConstraintY>(node, 1);

                stepVector.SetQuantityValue<NormalDisplacement>(node, averageNormalDisplacement);
                stepVector.SetQuantityValue<NormalDisplacement>(
                    node.ContactedNode,
                    averageNormalDisplacement
                );

                if (node.Type is NodeType.Neck)
                {
                    stepVector.SetQuantityValue<TangentialDisplacement>(
                        node,
                        GuessTangentialDisplacement(node)
                    );
                    stepVector.SetQuantityValue<TangentialDisplacement>(
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
