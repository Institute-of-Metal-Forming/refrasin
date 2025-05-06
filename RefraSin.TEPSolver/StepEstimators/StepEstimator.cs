using RefraSin.Coordinates;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;

namespace RefraSin.TEPSolver.StepEstimators;

class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(EquationSystem equationSystem)
    {
        Log.Logger.Debug("Estimate time step.");
        var map = new StepVectorMap(equationSystem);
        var vector = new StepVector(new double[map.TotalLength], map);
        FillStepVector(vector, equationSystem);
        return vector;
    }

    private static void FillStepVector(StepVector stepVector, EquationSystem equationSystem)
    {
        foreach (var constraint in equationSystem.Constraints)
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

        foreach (var particle in equationSystem.State.Particles)
        {
            foreach (var node in particle.Nodes)
            {
                var fluxToUpper = GuessFluxToUpper(node);
                var fluxBalance = fluxToUpper - GuessFluxToUpper(node.Lower);

                stepVector.SetQuantityValue<NormalDisplacement>(
                    node,
                    GuessNormalDisplacement(node, fluxBalance)
                );
                stepVector.SetQuantityValue<FluxToUpper>(node, fluxToUpper);
            }

            foreach (var neckNode in particle.Nodes.OfType<NeckNode>())
            {
                if (neckNode.Lower is GrainBoundaryNode)
                {
                    var grainBoundaryNode = neckNode.Lower;
                    var flux = stepVector.QuantityValue<FluxToUpper>(grainBoundaryNode);

                    for (int i = 0; i < 100; i++)
                    {
                        var normalDisplacement = GuessNormalDisplacement(
                            grainBoundaryNode,
                            2 * flux
                        );
                        var volumeBalance =
                            flux - neckNode.VolumeGradient.Normal * normalDisplacement;
                        var tangentialDisplacement =
                            volumeBalance / neckNode.VolumeGradient.Tangential;
                        var dissipation =
                            -neckNode.GibbsEnergyGradient.Normal * normalDisplacement
                            - neckNode.GibbsEnergyGradient.Tangential * tangentialDisplacement;
                        var newFlux =
                            Sign(dissipation)
                            * Sqrt(
                                neckNode.InterfaceDiffusionCoefficient.ToLower
                                    * Abs(dissipation)
                                    / (
                                        neckNode.Particle.VacancyVolumeEnergy
                                        * neckNode.SurfaceDistance.ToLower
                                    )
                            );

                        if (Abs((flux - newFlux) / flux) < 0.01)
                        {
                            stepVector.SetQuantityValue<FluxToUpper>(grainBoundaryNode, flux);
                            stepVector.SetQuantityValue<NormalDisplacement>(
                                grainBoundaryNode,
                                normalDisplacement
                            );
                            stepVector.SetQuantityValue<NormalDisplacement>(
                                neckNode,
                                normalDisplacement
                            );
                            stepVector.SetQuantityValue<TangentialDisplacement>(
                                neckNode,
                                tangentialDisplacement
                            );
                            break;
                        }

                        flux = newFlux;
                    }
                }
                else
                {
                    var grainBoundaryNode = neckNode.Upper;
                    var flux = -stepVector.QuantityValue<FluxToUpper>(neckNode);

                    for (int i = 0; i < 100; i++)
                    {
                        var normalDisplacement = GuessNormalDisplacement(
                            grainBoundaryNode,
                            2 * flux
                        );
                        var volumeBalance =
                            flux - neckNode.VolumeGradient.Normal * normalDisplacement;
                        var tangentialDisplacement =
                            volumeBalance / neckNode.VolumeGradient.Tangential;
                        var dissipation =
                            -neckNode.GibbsEnergyGradient.Normal * normalDisplacement
                            - neckNode.GibbsEnergyGradient.Tangential * tangentialDisplacement;
                        var newFlux =
                            Sign(dissipation)
                            * Sqrt(
                                neckNode.InterfaceDiffusionCoefficient.ToUpper
                                    * Abs(dissipation)
                                    / (
                                        neckNode.Particle.VacancyVolumeEnergy
                                        * neckNode.SurfaceDistance.ToUpper
                                    )
                            );

                        if (Abs((flux - newFlux) / flux) < 0.01)
                        {
                            stepVector.SetQuantityValue<FluxToUpper>(neckNode, -flux);
                            stepVector.SetQuantityValue<NormalDisplacement>(
                                grainBoundaryNode,
                                normalDisplacement
                            );
                            stepVector.SetQuantityValue<NormalDisplacement>(
                                neckNode,
                                normalDisplacement
                            );
                            stepVector.SetQuantityValue<TangentialDisplacement>(
                                neckNode,
                                tangentialDisplacement
                            );
                            break;
                        }

                        flux = newFlux;
                    }
                }
            }
        }

        foreach (var particle in equationSystem.State.Particles)
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

    private static double GuessFluxToUpper(NodeBase node) =>
        node.InterfaceDiffusionCoefficient.ToUpper
        * (GuessVacancyConcentration(node.Upper) - GuessVacancyConcentration(node))
        / node.SurfaceDistance.ToUpper;

    private static double GuessVacancyConcentration(NodeBase node)
    {
        var energyGradient = node is not NeckNode
            ? -node.GibbsEnergyGradient.Normal
            : Abs(node.GibbsEnergyGradient.Tangential);
        return energyGradient / node.SurfaceDistance.Sum * 2 / node.Particle.VacancyVolumeEnergy;
    }

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
