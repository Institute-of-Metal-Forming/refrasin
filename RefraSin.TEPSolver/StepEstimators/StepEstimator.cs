using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;

namespace RefraSin.TEPSolver.StepEstimators;

public class StepEstimator : IStepEstimator
{
    public StepVector EstimateStep(ISolverSession? solverSession, EquationSystem equationSystem)
    {
        Log.Logger.Debug("Estimate time step.");
        var map = new StepVectorMap(equationSystem);
        var vector = new StepVector(new double[equationSystem.Size], map);
        FillStepVector(vector, equationSystem);
        return vector;
    }

    private static void FillStepVector(StepVector stepVector, EquationSystem equationSystem)
    {
        foreach (var constraint in equationSystem.Items.OfType<IConstraint>())
        {
            stepVector.SetItemValue(
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

                stepVector.SetItemValue<NormalDisplacement>(
                    node,
                    GuessNormalDisplacement(node, fluxBalance)
                );
                stepVector.SetItemValue<FluxToUpper>(node, fluxToUpper);
            }

            foreach (var neckNode in particle.Nodes.OfType<NeckNode>())
            {
                if (neckNode.Lower is GrainBoundaryNode)
                {
                    var grainBoundaryNode = neckNode.Lower;
                    var flux = stepVector.ItemValue<FluxToUpper>(grainBoundaryNode);

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
                            stepVector.SetItemValue<FluxToUpper>(grainBoundaryNode, flux);
                            stepVector.SetItemValue<NormalDisplacement>(
                                grainBoundaryNode,
                                normalDisplacement
                            );
                            stepVector.SetItemValue<NormalDisplacement>(
                                neckNode,
                                normalDisplacement
                            );
                            stepVector.SetItemValue<TangentialDisplacement>(
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
                    var flux = -stepVector.ItemValue<FluxToUpper>(neckNode);

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
                            stepVector.SetItemValue<FluxToUpper>(neckNode, -flux);
                            stepVector.SetItemValue<NormalDisplacement>(
                                grainBoundaryNode,
                                normalDisplacement
                            );
                            stepVector.SetItemValue<NormalDisplacement>(
                                neckNode,
                                normalDisplacement
                            );
                            stepVector.SetItemValue<TangentialDisplacement>(
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
            var isXFixed = stepVector.StepVectorMap.HasItem<FixedParticleConstraintX>(particle);
            var isYFixed = stepVector.StepVectorMap.HasItem<FixedParticleConstraintY>(particle);

            if (!isXFixed || !isYFixed)
            {
                var grainBoundaryNode = particle.Nodes.OfType<GrainBoundaryNode>().First();
                var normalNodeDisplacement =
                    stepVector.ItemValue<NormalDisplacement>(grainBoundaryNode)
                    + stepVector.ItemValue<NormalDisplacement>(grainBoundaryNode.ContactedNode);

                if (!isXFixed)
                {
                    stepVector.SetItemValue<ParticleDisplacementX>(
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
                    stepVector.SetItemValue<ParticleDisplacementY>(
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
