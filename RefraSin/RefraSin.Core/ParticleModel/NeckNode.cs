using System;
using IMF.Coordinates.Polar;
using IMF.Maths;
using IMF.Utils;
using MathNet.Numerics;
using Microsoft.Extensions.Logging;
using RefraSin.Core.Solver;
using static System.Math;
using static MathNet.Numerics.Constants;
using static RefraSin.Core.Solver.InvalidityReason;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten welcher am Punkt des Sinterhalse liegt. Vermittelt den Übergang zwischen freien Oberflächen und Korngrenzen.
    /// </summary>
    public class NeckNode : ContactNode<NeckNode>, INeckNode
    {
        private readonly ILogger<NeckNode> _logger = Configuration.CreateLogger<NeckNode>();
        private readonly object _neckLock = new();
        private ToUpperToLower? _diffusionCoefficient;

        private double? _curvature;

        /// <inheritdoc />
        public NeckNode(Particle particle, PolarPoint coordinates) : base(particle, coordinates) { }

        public NeckNode OppositeNeckNode
        {
            get
            {
                if (Upper is GrainBoundaryNode)
                {
                    var upper = Upper;
                    while (upper is GrainBoundaryNode)
                    {
                        upper = upper.Upper;
                    }

                    return (NeckNode) upper;
                }
                else
                {
                    var lower = Lower;
                    while (lower is GrainBoundaryNode)
                    {
                        lower = lower.Lower;
                    }

                    return (NeckNode) lower;
                }
            }
        }

        public Guid OppositeNeckNodeId => OppositeNeckNode.Id;

        /// <inheritdoc />
        public override PolarVector TimeStepDisplacementVector => throw new NotSupportedException();

        public override double InterfaceEnergy => Particle.Material.SurfaceEnergy;

        public override ToUpperToLower DiffusionCoefficient => _diffusionCoefficient ??= new ToUpperToLower(
            Upper.DiffusionCoefficient.ToLower,
            Lower.DiffusionCoefficient.ToUpper
        );

        /// <summary>
        /// Krümmung der freien Oberfläche am Sinterhals. Benötigt gültige Referenzen auf die Nachbarknoten (<see cref="Node.Upper"/>, <see cref="Node.Lower"/>, <see cref="ContactNode{TContacted}.ContactedNode"/>).
        /// </summary>
        public override double Curvature
        {
            get
            {
                if (_curvature.HasValue)
                    return _curvature.Value;

                var delta = SurfaceRadiusAngle.Sum - Pi / 2;

                var surfaceDistance = Upper is SurfaceNode ? SurfaceDistance.ToUpper : SurfaceDistance.ToLower;

                var x = surfaceDistance * Cos(delta);
                var y = surfaceDistance * Sin(delta);

                _curvature = -(y + x * Trig.Cot(MaterialInterface.DihedralAngle)) / Pow(x, 2) /
                                 Pow(1 + Pow(Trig.Cot(MaterialInterface.DihedralAngle), 2), 1.5);
                return _curvature.Value;
            }
        }

        /// <summary>
        /// Spannung durch den Kontakt. Wird aus der <see cref="NeckCurvature"/> berechnet. Kann nicht explizit gesetzt werden. Benötigt gültige Referenzen auf die Nachbarknoten (<see cref="Node.Upper"/>, <see cref="Node.Lower"/>, <see cref="ContactNode{TContacted}.ContactedNode"/>).
        /// </summary>
        public override double ContactStress
        {
            get => SurfaceTension;
            set => throw new NotSupportedException();
        }

        #region TimeStep

        /// <inheritdoc />
        public override void CalculateTimeStep(ISinteringSolverSession session)
        {
            if (Upper is GrainBoundaryNode) // wechsle zu anderem Teilchen, wenn unterer Hals 
            {
                ContactedNode.CalculateTimeStep(session);
                return;
            }

            lock (_neckLock)
            {
                InitFutureCoordinates();

                var volumeChange1 = DiffusionalFlowBalance * session.TimeStepWidth;
                var volumeChange2 = ContactedNode.DiffusionalFlowBalance * session.TimeStepWidth;
                var goalVolume1 = FutureNeighborElementsVolume + volumeChange1;
                var goalVolume2 = ContactedNode.FutureNeighborElementsVolume + volumeChange2;

                MergeFutureCoordinates();

                for (int i = 0;
                    !_logger.LogIfMaximumIterationCountReached("neck node shifting", i, session.SolverOptions.MaxIterationCount, LogLevel.Error);
                    i++)
                {
                    DoTangentialShifting(goalVolume1, goalVolume2);

                    DoNormalShifting(goalVolume1, goalVolume2);

                    // break condition
                    if (Abs(goalVolume1 - FutureNeighborElementsVolume) <
                        session.SolverOptions.IterationPrecision * Abs(volumeChange1) &&
                        Abs(goalVolume2 - ContactedNode.FutureNeighborElementsVolume) <
                        session.SolverOptions.IterationPrecision * Abs(volumeChange2))
                    {
                        _logger.LogTrace("Neck node {Id} shifting successful after {Iterations} iterations.", Id.ToShortString(), i);
                        return;
                    }
                }

                throw new InvalidTimeStepException(this, NotConverged);
            }
        }

        private void DoTangentialShifting(double goalVolume1, double goalVolume2)
        {
            var delta1 = PI - FutureSurfaceRadiusAngle.Sum;
            var delta2 = PI - ContactedNode.FutureSurfaceRadiusAngle.Sum;

            var dsTan1 = 2 * (goalVolume1 - FutureNeighborElementsVolume) / FutureSurfaceDistance.ToUpper /
                         Sin(delta1);
            var dsTan2 = 2 * (goalVolume2 - ContactedNode.FutureNeighborElementsVolume) /
                         ContactedNode.FutureSurfaceDistance.ToLower /
                         Sin(delta2);
            var dsTan = (dsTan1 + dsTan2) / 2;

            // time step validation
            if (
                dsTan > 0.5 * SurfaceDistance.ToUpper ||
                -dsTan > 0.5 * SurfaceDistance.ToLower ||
                dsTan > 0.5 * ContactedNode.SurfaceDistance.ToLower ||
                -dsTan > 0.5 * ContactedNode.SurfaceDistance.ToUpper
            )
                throw new InvalidTimeStepException(this, MovementToLarge);

            var psi1 = delta1 + FutureSurfaceRadiusAngle.ToUpper;
            var r = CosLaw.C(dsTan, FutureCoordinates.R, psi1);
            var dPhi = SinLaw.Alpha(dsTan, r, psi1);

            FutureCoordinates.R = r;
            FutureCoordinates.Phi += dPhi;
            ClearFutureGeometryCache();

            PullContactedNodesFutureCoordinates();
        }

        private void DoNormalShifting(double goalVolume1, double goalVolume2)
        {
            var dsNor = -(goalVolume1 - FutureNeighborElementsVolume - goalVolume2 + ContactedNode.FutureNeighborElementsVolume) /
                        (FutureSurfaceDistance.ToLower + FutureSurfaceDistance.ToUpper * Sin(FutureSurfaceRadiusAngle.Sum - PiOver2));

            var psi = PiOver2 - FutureSurfaceRadiusAngle.ToLower;
            var r = CosLaw.C(dsNor, FutureCoordinates.R, psi);
            var dPhi = SinLaw.Alpha(dsNor, r, psi);

            FutureCoordinates.R = r;
            FutureCoordinates.Phi += dPhi;
            ClearFutureGeometryCache();

            PullContactedNodesFutureCoordinates();
        }

        /// <inheritdoc />
        public override void ValidateTimeStep(ISinteringSolverSession session)
        {
            if (
                Abs(AngleDistance.ToUpper - FutureAngleDistance.ToUpper) > 0.2 * AngleDistance.ToUpper ||
                Abs(ContactedNode.AngleDistance.ToLower - ContactedNode.FutureAngleDistance.ToLower) >
                0.2 * ContactedNode.AngleDistance.ToLower
            )
            {
                throw new InvalidTimeStepException(this, MovementToLarge);
            }
        }

        #endregion

        #region Remeshing

        public void RemeshNeighborhood(ISinteringSolverSession session)
        {
            RemoveUpperIfTooClose(session);
            RemoveLowerIfTooClose(session);
            InsertAboveIfUpperToFar(session);
            InsertBelowIfLowerToFar(session);
        }

        private void RemoveUpperIfTooClose(ISinteringSolverSession session)
        {
            if (SurfaceDistance.ToUpper < session.SolverOptions.RemeshingDistanceDeletionLimit * session.SolverOptions.DiscretizationWidth)
                if (Upper.Upper is not NeckNode) // preserve the only grain boundary node of a neck
                    RemoveNode(Upper);
        }

        private void RemoveLowerIfTooClose(ISinteringSolverSession session)
        {
            if (SurfaceDistance.ToLower < session.SolverOptions.RemeshingDistanceDeletionLimit * session.SolverOptions.DiscretizationWidth)
                if (Lower.Lower is not NeckNode) // preserve the only grain boundary node of a neck
                    RemoveNode(Lower);
        }

        private void RemoveNode(Node node)
        {
            if (node is GrainBoundaryNode grainBoundaryNode)
            {
                grainBoundaryNode.ContactedNode.Remove();
                grainBoundaryNode.Disconnect();
            }

            node.Remove();
        }

        private void InsertAboveIfUpperToFar(ISinteringSolverSession session)
        {
            if (Upper is GrainBoundaryNode grainBoundaryNode)
            {
                if (session.SolverOptions.AddAdditionalGrainBoundaryNodes)
                {
                    if (SurfaceDistance.ToUpper > session.SolverOptions.RemeshingDistanceInsertionLimit * session.SolverOptions.DiscretizationWidth *
                        session.SolverOptions.GrainBoundaryRemeshingInsertionRatio)
                    {
                        var newNode = CreateIntermediateGrainBoundaryNode(grainBoundaryNode);
                        InsertAbove(newNode);
                        ContactedNode.InsertBelow(newNode.ContactedNode);
                    }
                }
            }
            else
            {
                if (SurfaceDistance.ToUpper > session.SolverOptions.RemeshingDistanceInsertionLimit * session.SolverOptions.DiscretizationWidth)
                {
                    var newNode = CreateIntermediateSurfaceNode(Upper);
                    InsertAbove(newNode);
                }
            }
        }

        private void InsertBelowIfLowerToFar(ISinteringSolverSession session)
        {
            if (Lower is GrainBoundaryNode grainBoundaryNode)
            {
                if (SurfaceDistance.ToLower > session.SolverOptions.RemeshingDistanceInsertionLimit * session.SolverOptions.DiscretizationWidth *
                    session.SolverOptions.GrainBoundaryRemeshingInsertionRatio)
                {
                    if (session.SolverOptions.AddAdditionalGrainBoundaryNodes)
                    {
                        var newNode = CreateIntermediateGrainBoundaryNode(grainBoundaryNode);
                        InsertBelow(newNode);
                        ContactedNode.InsertAbove(newNode.ContactedNode);
                    }
                }
            }
            else
            {
                if (SurfaceDistance.ToLower > session.SolverOptions.RemeshingDistanceInsertionLimit * session.SolverOptions.DiscretizationWidth)
                {
                    var newNode = CreateIntermediateSurfaceNode(Lower);
                    InsertBelow(newNode);
                }
            }
        }

        private GrainBoundaryNode CreateIntermediateGrainBoundaryNode(GrainBoundaryNode farNode)
        {
            var pointHalfWayTo = Coordinates.PointHalfWayTo(farNode.Coordinates);
            var newNode = new GrainBoundaryNode(Particle, pointHalfWayTo);
            var newOthersNode = new GrainBoundaryNode(ContactedNode.Particle, new PolarPoint(pointHalfWayTo,
                ContactedNode.Particle.LocalCoordinateSystem));

            var pastContactStress = (farNode.PastContactStress + PastContactStress) / 2;
            newNode.PastContactStress = pastContactStress;
            newOthersNode.PastContactStress = pastContactStress;

            newNode.Connect(newOthersNode);
            return newNode;
        }

        private SurfaceNode CreateIntermediateSurfaceNode(Node farNode)
        {
            var pointHalfWayTo = Coordinates.PointHalfWayTo(farNode.Coordinates);
            return new SurfaceNode(Particle, pointHalfWayTo);
        }

        #endregion

        /// <inheritdoc />
        protected override void ClearGeometryCache()
        {
            _curvature = null;
            base.ClearGeometryCache();
        }
    }
}