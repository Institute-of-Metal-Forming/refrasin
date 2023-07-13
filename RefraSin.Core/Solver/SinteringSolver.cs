using System;
using System.Linq;
using System.Threading;
using IMF.Coordinates;
using IMF.Coordinates.Polar;
using IMF.Iteration;
using MathNet.Numerics;
using MathNet.Numerics.Optimization;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;
using RefraSin.Core.Solver.Solution;
using static System.Math;
using static IMF.Iteration.InterceptReason;

namespace RefraSin.Core.Solver
{
    /// <summary>
    ///     Lösungsalgorithmus für den Sinterprozess.
    /// </summary>
    public static partial class SinteringSolver
    {
        /// <summary>
        ///     Hauptfunktion für das Lösen des Sintermodells.
        /// </summary>
        /// <returns>true wenn erfolgreich, sonst false</returns>
        public static ISinteringSolverSolution Solve(ISinteringProcess process, SinteringSolverOptions options, CancellationToken cancellationToken)
        {
            var session = Session.CreateSessionFromProcess(process, options);
            session.Start();

            try
            {
                RunTimeIntegrationLoop(session, cancellationToken);
            }
            catch (Exception e)
            {
                session.EndFailed(e);
                return new SolutionResult(false, session.TimeSeries, process);
            }

            session.End();

            return new SolutionResult(true, session.TimeSeries, process);
        }

        private static void RunTimeIntegrationLoop(Session session, CancellationToken cancellationToken)
        {
            while (EndTimeIsNotReached(session))
            {
                CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid(session, cancellationToken);

                session.ApplyTimeStep();
                session.IncreaseTimeStepWidthConditionally();
            }
        }

        private static bool EndTimeIsNotReached(Session session) => !(session.CurrentTime > session.EndTime);

        private static void CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid(Session session, CancellationToken cancellationToken)
        {
            for (var i = 0;; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                session.ThrowIfTimedOut();

                if (i > session.SolverOptions.MaxIterationCount)
                    throw new CriticalIterationInterceptedException(nameof(CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid),
                        MaxIterationCountExceeded, i);

                if (TryCalculateTimeStep(session))
                    return;

                session.DecreaseTimeStepWidthConditionally();
            }
        }

        private static bool TryCalculateTimeStep(Session session)
        {
            try
            {
                CalculateTimeStep(session);
            }
            catch (InvalidTimeStepException e)
            {
                session.LogInvalidTimeStep(e);
                return false;
            }
            catch (UncriticalIterationInterceptedException e)
            {
                session.LogUncriticalIterationIntercepted(e);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Berechne Zustand nach neuem Zeitschritt.
        /// </summary>
        private static void CalculateTimeStep(Session session)
        {
            InitParticleFuturePlacement(session.Particles.Root);

            RemeshParticleSurfaces(session);
            CalculateTimeStepsOnFreeSurfaces(session);
            CalculateTimeStepsOnGrainBoundaries(session);
            ValidateTimeSteps(session);
        }

        private static void InitParticleFuturePlacement(Particle particle)
        {
            particle.FutureCenterCoordinates = particle.CenterCoordinates.Clone();
            particle.FutureRotationAngle = particle.RotationAngle;
        }

        private static void RemeshParticleSurfaces(Session session)
        {
            foreach (var particle in session.Particles)
            {
                foreach (var neckNode in particle.Surface.OfType<NeckNode>().ToArray())
                    neckNode.RemeshNeighborhood(session);
                foreach (var neckNode in particle.Surface.OfType<NeckNode>().ToArray())
                    neckNode.RemeshNeighborhood(session);
                foreach (var surfaceNode in particle.Surface.OfType<SurfaceNode>().ToArray())
                    surfaceNode.RemoveSelfIfNeighborsToClose(session);
            }
        }

        private static void CalculateTimeStepsOnFreeSurfaces(Session session)
        {
            foreach (var particle in session.Particles)
            foreach (var surfaceNode in particle.Surface.OfType<SurfaceNode>())
                surfaceNode.CalculateTimeStep(session);
        }

        private static void CalculateTimeStepsOnGrainBoundaries(Session session)
        {
            session.Particles.ForEach(parent =>
            {
                foreach (var child in parent.Children)
                    CalculateTimeStepOfParticlePair(parent, child, session);
            });
        }

        private static void ValidateTimeSteps(Session session)
        {
            session.Particles.ForEach(particle =>
            {
                foreach (var surfaceNode in particle.Surface)
                    surfaceNode.ValidateTimeStep(session);
            });
        }

        private static void CalculateTimeStepOfParticlePair(Particle parent, Particle child, Session session)
        {
            var grainBoundaryNodes = SelectGrainBoundaryNodesInContactToChild(parent, child);
            var neckNodes = SelectNeckNodesInContactToChild(parent, child);

            InitParticleFuturePlacement(child);

            InitFutureCoordinatesOfNeckNodes(neckNodes);

            CalculateChildsMovement(child, grainBoundaryNodes, session);

            CalculateTimeStepsOfNecks(neckNodes, session);
        }

        private static GrainBoundaryNode[] SelectGrainBoundaryNodesInContactToChild(Particle parent, Particle child) =>
            parent.Surface.OfType<GrainBoundaryNode>().Where(x => x.ContactedNode.Particle == child).ToArray();

        private static NeckNode[] SelectNeckNodesInContactToChild(Particle parent, Particle child) =>
            parent.Surface.OfType<NeckNode>().Where(x => x.ContactedNode.Particle == child).ToArray();

        private static void InitFutureCoordinatesOfNeckNodes(NeckNode[] neckNodes)
        {
            foreach (var node in neckNodes)
                node.InitFutureCoordinates();
        }

        private static void CalculateTimeStepsOfNecks(NeckNode[] neckNodes, Session session)
        {
            foreach (var node in neckNodes)
                node.CalculateTimeStep(session);
        }

        private static void CalculateChildsMovement(Particle child, GrainBoundaryNode[] grainBoundaryNodes, Session session)
        {
            if (grainBoundaryNodes.Length == 1)
            {
                // iteration structure can be skipped
                DetermineChildsMovementFromOnlyGrainBoundaryNode(child, grainBoundaryNodes[0], session);
            }
            else
            {
                DetermineChildsMovementByMeanOfAllGrainBoundaryNodes(child, grainBoundaryNodes, session);
            }
        }

        private static void DetermineChildsMovementFromOnlyGrainBoundaryNode(Particle child, GrainBoundaryNode grainBoundaryNode, Session session)
        {
            grainBoundaryNode.ContactStress = 0;

            grainBoundaryNode.CalculateTimeStep(session);
            grainBoundaryNode.ContactedNode.CalculateTimeStep(session);

            var movementVector = CalculateGrainBoundaryDisplacementVector(grainBoundaryNode);
            child.FutureCenterCoordinates = child.CenterCoordinates + movementVector;
        }

        private static void DetermineChildsMovementByMeanOfAllGrainBoundaryNodes(Particle child, GrainBoundaryNode[] grainBoundaryNodes,
            Session session)
        {
            PolarVector? oldMovementVector = null;

            for (var i = 0;; i++)
            {
                if (i > session.SolverOptions.MaxIterationCount)
                    throw new CriticalIterationInterceptedException($"{nameof(CalculateChildsMovement)} of {child}",
                        MaxIterationCountExceeded, i);

                var movementVector = GuessChildsMovementByMeanOfAllGrainBoundaryNodes(child, grainBoundaryNodes, session);

                session.LogChildsMovementDetermined(child, movementVector);

                if (
                    oldMovementVector != null &&
                    MovementVectorDidNotChangeWithinPrecision(movementVector, oldMovementVector, session)
                )
                {
                    session.LogChildsFutureCoordinates(child, i);
                    return;
                }

                oldMovementVector = movementVector;
            }
        }

        private static PolarVector GuessChildsMovementByMeanOfAllGrainBoundaryNodes(Particle child, GrainBoundaryNode[] grainBoundaryNodes,
            Session session)
        {
            _ = TryCalculateGrainBoundaryStresses(grainBoundaryNodes, session);

            var direction = CalculateMovementDirection(grainBoundaryNodes);

            var distance = FindDistanceOfGrainBoundaryStressMinimum(child, grainBoundaryNodes, direction, session);
            child.MovementDistanceOfLastTimeStep = distance;

            var movementVector = direction * distance;
            child.FutureCenterCoordinates = child.CenterCoordinates + movementVector;
            return movementVector;
        }

        private static PolarVector CalculateMovementDirection(GrainBoundaryNode[] grainBoundaryNodes)
        {
            var vectors = CalculateGrainBoundaryDisplacementVectors(grainBoundaryNodes);

            var direction = vectors.AverageDirection();
            return direction;
        }

        private static PolarVector CalculateGrainBoundaryDisplacementVector(GrainBoundaryNode grainBoundaryNode) =>
            new(grainBoundaryNode.TimeStepDisplacementVector.Absolute - grainBoundaryNode.ContactedNode.TimeStepDisplacementVector.Absolute,
                grainBoundaryNode.Particle.LocalCoordinateSystem);

        private static PolarVector[] CalculateGrainBoundaryDisplacementVectors(GrainBoundaryNode[] grainBoundaryNodes) =>
            grainBoundaryNodes.Select(CalculateGrainBoundaryDisplacementVector).ToArray();

        private static double FindDistanceOfGrainBoundaryStressMinimum(Particle child, GrainBoundaryNode[] grainBoundaryNodes,
            PolarVector direction, Session session)
        {
            var (lowerBound, upperBound) = GuessMinimizationBounds(child, session);
            var tolerance = session.SolverOptions.IterationPrecision / 2 *
                            Abs(child.MovementDistanceOfLastTimeStep ?? session.SolverOptions.DiscretizationWidth / 10);
            var minimizedFunction = CreateStressIntegralFunction(child, grainBoundaryNodes, direction, session);

            var minimum = GoldenSectionMinimizer.Minimum(
                minimizedFunction,
                lowerBound, upperBound, tolerance,
                session.SolverOptions.MaxIterationCount,
                session.SolverOptions.MaxIterationCount
            );

            return minimum.MinimizingPoint;
        }

        private static IScalarObjectiveFunction CreateStressIntegralFunction(Particle child, GrainBoundaryNode[] grainBoundaryNodes,
            PolarVector direction, Session session)
        {
            double StressIntegralFunction(double distance)
            {
                child.FutureCenterCoordinates = child.CenterCoordinates + direction * distance;

                _ = TryCalculateGrainBoundaryStresses(grainBoundaryNodes, session);
                var stressIntegral = IntegrateGrainBoundaryStresses(grainBoundaryNodes);

                if (stressIntegral.IsFinite() == false)
                    throw new UncriticalIterationInterceptedException(nameof(FindDistanceOfGrainBoundaryStressMinimum), InvalidStateOccured);

                return stressIntegral;
            }

            return ObjectiveFunction.ScalarValue(StressIntegralFunction);
        }

        private static (double lower, double upper) GuessMinimizationBounds(Particle child, Session session)
        {
            if (child.MovementDistanceOfLastTimeStep.HasValue)
            {
                var windowAroundLastDistance = Abs(0.01 * child.MovementDistanceOfLastTimeStep.Value *
                                                   (session.TimeStepWidth / session.TimeStepWidthOfLastStep ?? 1));
                return (child.MovementDistanceOfLastTimeStep.Value - windowAroundLastDistance,
                    child.MovementDistanceOfLastTimeStep.Value + windowAroundLastDistance);
            }

            return (-0.01 * session.SolverOptions.DiscretizationWidth, +0.01 * session.SolverOptions.DiscretizationWidth);
        }

        private static bool MovementVectorDidNotChangeWithinPrecision(PolarVector movementVector, PolarVector oldMovementVector, Session session) =>
            Abs((movementVector - oldMovementVector).Norm) < Abs(session.SolverOptions.IterationPrecision * movementVector.Norm * 10);

        private static bool TryCalculateGrainBoundaryStresses(GrainBoundaryNode[] grainBoundaryNodes, Session session)
        {
            try
            {
                CalculateGrainBoundaryStresses(grainBoundaryNodes, session);
            }
            catch (UncriticalIterationInterceptedException e)
            {
                session.LogUncriticalIterationIntercepted(e);
                return false;
            }

            return true;
        }

        private static void CalculateGrainBoundaryStresses(GrainBoundaryNode[] grainBoundaryNodes, Session session)
        {
            InitContactStressesOfGrainBoundaryNodes(grainBoundaryNodes);

            ModifyGrainBoundaryStressesUntilGrainBoundaryIsCompact(grainBoundaryNodes, session);
        }

        private static void InitContactStressesOfGrainBoundaryNodes(GrainBoundaryNode[] grainBoundaryNodes)
        {
            foreach (var node in grainBoundaryNodes)
            {
                node.ContactStress = node.PastContactStress;
                node.ContactedNode.ContactStress = node.PastContactStress;
            }
        }

        private static void ModifyGrainBoundaryStressesUntilGrainBoundaryIsCompact(GrainBoundaryNode[] grainBoundaryNodes, Session session)
        {
            int i;
            for (i = 0;; i++)
            {
                if (i > session.SolverOptions.MaxIterationCount)
                    throw new UncriticalIterationInterceptedException(nameof(ModifyGrainBoundaryStressesUntilGrainBoundaryIsCompact),
                        MaxIterationCountExceeded, i);

                CalculateTimeStepsOfGrainBoundaryNodes(grainBoundaryNodes, session);
                var childsNodesSelfVolumes = CalculateFutureElementsVolumesOfGrainBoundaryNodes(grainBoundaryNodes);

                PullFutureCoordinatesOfChildsGrainBoundaryNodes(grainBoundaryNodes);
                var childsNodesPulledVolumes = CalculateFutureElementsVolumesOfGrainBoundaryNodes(grainBoundaryNodes);

                var volumeDifferences = CalculateElementsVolumeDifferencesOfGrainBoundaryNodes(childsNodesSelfVolumes, childsNodesPulledVolumes);

                if (VolumeDifferencesAreSmallEnough(grainBoundaryNodes, volumeDifferences, session))
                {
                    session.LogGrainBoundaryStressesDetermined(i);
                    return;
                }

                var stressChanges = CalculateStressChangesOfGrainBoundaryNodes(grainBoundaryNodes, volumeDifferences, i, session);

                ApplyStressChangesOnGrainBoundaryNodes(grainBoundaryNodes, stressChanges);
            }
        }

        private static double IntegrateGrainBoundaryStresses(GrainBoundaryNode[] grainBoundaryNodes) =>
            grainBoundaryNodes.Select(k => Pow(k.ContactStress, 2) * k.SurfaceDistance.Sum).Sum() / 2;

        private static void CalculateTimeStepsOfGrainBoundaryNodes(GrainBoundaryNode[] grainBoundaryNodes, Session session)
        {
            foreach (var node in grainBoundaryNodes)
            {
                node.CalculateTimeStep(session);
                node.ContactedNode.CalculateTimeStep(session);
            }
        }

        private static double[] CalculateFutureElementsVolumesOfGrainBoundaryNodes(GrainBoundaryNode[] grainBoundaryNodes) =>
            grainBoundaryNodes.Select(k => k.ContactedNode.FutureNeighborElementsVolume)
                .ToArray();

        private static void PullFutureCoordinatesOfChildsGrainBoundaryNodes(GrainBoundaryNode[] grainBoundaryNodes)
        {
            foreach (var node in grainBoundaryNodes)
                node.PullContactedNodesFutureCoordinates();
        }

        private static double[] CalculateElementsVolumeDifferencesOfGrainBoundaryNodes(double[] childsNodesSelfVolumes,
            double[] childsNodesPulledVolumes) =>
            childsNodesPulledVolumes.Zip(childsNodesSelfVolumes, (pulled, self) => pulled - self).ToArray();

        private static bool VolumeDifferencesAreSmallEnough(GrainBoundaryNode[] grainBoundaryNodes, double[] volumeDifferences, Session session)
        {
            var averageVolumeChange = grainBoundaryNodes
                .Select(k => Abs(k.NeighborElementsVolume - k.FutureNeighborElementsVolume)).Average();

            if (volumeDifferences.All(v => Abs(v) < session.SolverOptions.IterationPrecision * averageVolumeChange))
                return true;

            return false;
        }

        private static double[] CalculateStressChangesOfGrainBoundaryNodes(GrainBoundaryNode[] grainBoundaryNodes,
            double[] volumeDifferences, int iterationCount, Session session)
        {
            var grainBoundaryTensionBrakeFactor =
                Pow((session.SolverOptions.MaxIterationCount - (double)iterationCount) / session.SolverOptions.MaxIterationCount, 2)
              / Pow(grainBoundaryNodes.Length, 0.5);

            var stressChanges =
                CalculateStressChangesWithConstantNeighborsAssumed(grainBoundaryNodes, volumeDifferences, grainBoundaryTensionBrakeFactor, session);

            if (iterationCount.IsEven()) return ModifyStressChangesWithRespectToNeighbors(grainBoundaryNodes, stressChanges, session);

            return stressChanges;
        }

        private static double[] CalculateStressChangesWithConstantNeighborsAssumed(GrainBoundaryNode[] grainBoundaryNodes, double[] volumeDifferences,
            double grainBoundaryTensionBrakeFactor,
            Session session)
        {
            var stressChanges = grainBoundaryNodes.Zip(volumeDifferences,
                (node, diff) =>
                {
                    return grainBoundaryTensionBrakeFactor * node.CalculateContactStressChangeFromFailVolume(diff, session.TimeStepWidth) / 5;
                }).ToArray();
            return stressChanges;
        }

        private static double[] ModifyStressChangesWithRespectToNeighbors(GrainBoundaryNode[] grainBoundaryNodes, double[] stressChanges,
            Session session)
        {
            var modifiedStressChanges = stressChanges.ToArray();

            for (var k = 0; k < 5; k++)
            for (var j = 0; j < stressChanges.Length; j++)
            {
                var upperStressChange = j == stressChanges.Length - 1 ? 0 : modifiedStressChanges[j + 1];
                var lowerStressChange = j == 0 ? 0 : modifiedStressChanges[j - 1];
                var neighborInfluence = (upperStressChange * grainBoundaryNodes[j].SurfaceDistance.ToLower +
                                         lowerStressChange * grainBoundaryNodes[j].SurfaceDistance.ToUpper) /
                                        grainBoundaryNodes[j].SurfaceDistance.Sum;

                modifiedStressChanges[j] =
                    stressChanges[j] + session.SolverOptions.GrainBoundaryStressChangeNeighborInfluenceBreakFactor * neighborInfluence;
            }

            return modifiedStressChanges;
        }

        private static void ApplyStressChangesOnGrainBoundaryNodes(GrainBoundaryNode[] grainBoundaryNodes, double[] stressChanges)
        {
            foreach (var t in grainBoundaryNodes.Zip(stressChanges, (node, stressChange) => (node, stressChange)))
            {
                t.node.ContactStress += t.stressChange;
                t.node.ContactedNode.ContactStress = t.node.ContactStress;
            }
        }
    }
}