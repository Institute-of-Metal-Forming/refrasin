using RefraSin.Coordinates;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
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
        foreach (var constraint in stepVector.StepVectorMap.Constraints)
        {
            stepVector.SetConstraintLambdaValue(
                constraint,
                constraint switch
                {
                    FixedParticleConstraintX or FixedParticleConstraintY => 0,
                    _ => 1,
                }
            );
        }

        foreach (var particle in currentState.Particles)
        {
            foreach (var node in particle.Nodes)
            {
                if (node.Type is not NodeType.Neck)
                {
                    var fluxToUpper = GuessFluxToUpper(node);
                    var fluxBalance = fluxToUpper - GuessFluxToUpper(node.Lower);

                    stepVector.SetQuantityValue<NormalDisplacement>(
                        node,
                        GuessNormalDisplacement(node, fluxBalance)
                    );
                    stepVector.SetQuantityValue<FluxToUpper>(node, fluxToUpper);
                }
            }

            foreach (var neckNode in particle.Nodes.OfType<NeckNode>())
            {
                var fluxToUpper = GuessFluxToUpper(neckNode);
                var fluxBalance = fluxToUpper - GuessFluxToUpper(neckNode.Lower);

                if (neckNode.Lower is GrainBoundaryNode)
                {
                    stepVector.SetQuantityValue<NormalDisplacement>(
                        neckNode,
                        stepVector.QuantityValue<NormalDisplacement>(neckNode.Lower)
                    );
                }
                else
                {
                    stepVector.SetQuantityValue<NormalDisplacement>(
                        neckNode,
                        stepVector.QuantityValue<NormalDisplacement>(neckNode.Upper)
                    );
                }

                stepVector.SetQuantityValue<TangentialDisplacement>(
                    neckNode,
                    GuessTangentialDisplacement(neckNode, fluxBalance, stepVector)
                );
                stepVector.SetQuantityValue<FluxToUpper>(neckNode, fluxToUpper);
            }
        }

        foreach (var particle in currentState.Particles)
        {
            var isXFixed = stepVector.StepVectorMap.HasConstraint<FixedParticleConstraintX>(
                particle
            );
            var isYFixed = stepVector.StepVectorMap.HasConstraint<FixedParticleConstraintY>(
                particle
            );

            if (!isXFixed || !isYFixed)
            {
                var grainBoundaryNode = particle.Nodes.OfType<GrainBoundaryNode>().First();
                var normalNodeDisplacement =
                    stepVector.QuantityValue<NormalDisplacement>(grainBoundaryNode)
                    + stepVector.QuantityValue<NormalDisplacement>(grainBoundaryNode.ContactedNode);

                if (!isXFixed)
                {
                    stepVector.SetQuantityValue<ParticleDisplacementX>(
                        particle,
                        Cos(
                            grainBoundaryNode.Particle.RotationAngle
                                + grainBoundaryNode.Coordinates.Phi
                                + grainBoundaryNode.RadiusNormalAngle.ToLower
                        ) * normalNodeDisplacement
                    );
                }

                if (!isYFixed)
                {
                    stepVector.SetQuantityValue<ParticleDisplacementY>(
                        particle,
                        Sin(
                            grainBoundaryNode.Particle.RotationAngle
                                + grainBoundaryNode.Coordinates.Phi
                                + grainBoundaryNode.RadiusNormalAngle.ToLower
                        ) * normalNodeDisplacement
                    );
                }
            }
        }
    }

    private static double GuessNormalDisplacement(NodeBase node, double fluxBalance)
    {
        var displacement = -fluxBalance / node.VolumeGradient.Normal;
        return displacement;
    }

    private static double GuessTangentialDisplacement(
        NodeBase node,
        double fluxBalance,
        StepVector stepVector
    )
    {
        var displacement =
            -(
                fluxBalance
                + stepVector.QuantityValue<NormalDisplacement>(node) * node.VolumeGradient.Normal
            ) / node.VolumeGradient.Tangential;
        return displacement;
    }

    private static double GuessFluxToUpper(NodeBase node) =>
        node.InterfaceDiffusionCoefficient.ToUpper
        * (GuessVacancyConcentration(node.Upper) - GuessVacancyConcentration(node))
        / node.SurfaceDistance.ToUpper;

    private static double GuessVacancyConcentration(NodeBase node)
    {
        var energyGradient = node is not NeckNode
            ? -node.GibbsEnergyGradient.Normal
            : 20 * Abs(node.GibbsEnergyGradient.Tangential) - node.GibbsEnergyGradient.Normal;
        return energyGradient / node.SurfaceDistance.Sum * 2 / node.Particle.VacancyVolumeEnergy;
    }

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
