using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
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
                if (node is not INodeContact)
                {
                    stepVector.LambdaVolume(node, 0);
                    stepVector.FluxToUpper(node, GuessFluxToUpper(node));
                    stepVector.NormalDisplacement(node, GuessNormalDisplacement(node));
                }
            }
        }

        foreach (var contact in currentState.ParticleContacts)
        {
            var averageNormalDisplacement = contact.FromNodes.OfType<GrainBoundaryNode>().Average(GuessNormalDisplacement) +
                                            contact.ToNodes.OfType<GrainBoundaryNode>().Average(GuessNormalDisplacement);
            stepVector.RadialDisplacement(contact, averageNormalDisplacement);
            stepVector.AngleDisplacement(contact, 0);
            stepVector.RotationDisplacement(contact, 0);
            stepVector.LambdaContactRotation(contact, 1);

            foreach (var node in contact.FromNodes)
            {
                stepVector.LambdaContactDistance(node, 0);
                stepVector.LambdaContactDirection(node, 0);
                
                stepVector.LambdaVolume(node, 0);
                stepVector.FluxToUpper(node, GuessFluxToUpper(node));
                stepVector.NormalDisplacement(node, averageNormalDisplacement);

                stepVector.LambdaVolume(node.ContactedNode, 0);
                stepVector.FluxToUpper(node.ContactedNode, GuessFluxToUpper(node.ContactedNode));
                stepVector.NormalDisplacement(node.ContactedNode, averageNormalDisplacement);

                if (node is NeckNode)
                {
                    stepVector.TangentialDisplacement(node, GuessTangentialDisplacement(node));
                    stepVector.TangentialDisplacement(node.ContactedNode, GuessTangentialDisplacement(node.ContactedNode));
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
        -node.InterfaceDiffusionCoefficient.ToUpper * (GuessVacancyConcentration(node) - GuessVacancyConcentration(node.Upper))
      / Pow(node.SurfaceDistance.ToUpper, 2);

    private static double GuessVacancyConcentration(NodeBase node) =>
        (node is not NeckNode ? node.GibbsEnergyGradient.Normal : -Abs(node.GibbsEnergyGradient.Tangential))
      / node.Particle.VacancyVolumeEnergy;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}