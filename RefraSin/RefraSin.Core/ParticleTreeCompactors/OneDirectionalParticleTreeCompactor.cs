using System.Collections.Generic;
using System.Linq;
using IMF.Coordinates;
using IMF.Coordinates.Polar;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.Solver;

namespace RefraSin.Core.ParticleTreeCompactors
{
    /// <summary>
    /// Provides an algorithm for particle tree compaction, that draws all particles in direction of their parents until contact is met.
    /// </summary>
    public class OneDirectionalParticleTreeCompactor : IParticleTreeCompactor
    {
        /// <summary>
        /// Ratio of the step with of translation to the discretization width. Higher values mean smaller step widths.
        /// </summary>
        public int DiscretizationWidthDistanceStepRatio { get; set; } = 10;

        /// <inheritdoc />
        public void Compact(ISinteringSolverSession session)
        {
            var distanceStep = session.SolverOptions.DiscretizationWidth / DiscretizationWidthDistanceStepRatio;

            foreach (var parent in session.Particles)
            {
                foreach (var child in parent.Children)
                {
                    while (true)
                    {
                        var differences = parent.Surface.Select(k =>
                        {
                            var virtualNodesCoordinates = new PolarPoint(k.Coordinates, child.LocalCoordinateSystem);
                            return (node: k, diff: virtualNodesCoordinates.R - child.Surface.InterpolatedRadius(virtualNodesCoordinates.Phi));
                        }).ToArray();

                        var roots = GetDistanceRoots(differences).ToArray();

                        if (roots.Length > 0) // Paarweise Nullstellen am Parent vorhanden
                        {
                            foreach (var root in roots)
                            {
                                // unteren Halsknoten des Parents und zugehörigen des childs erzeugen
                                var parentsLowerNeckNode = new NeckNode(parent,
                                    new PolarPoint(root.first.angle, parent.Surface.InterpolatedRadius(root.first.angle),
                                        parent.LocalCoordinateSystem)
                                );
                                var childsUpperNeckNode = new NeckNode(child,
                                    new PolarPoint(parentsLowerNeckNode.Coordinates, child.LocalCoordinateSystem)
                                );
                                root.first.node.InsertBelow(parentsLowerNeckNode);
                                var childsUpperSurfaceNode =
                                    child.Surface.GetNearestNodesToAngle(childsUpperNeckNode.Coordinates.Phi).Lower;
                                childsUpperSurfaceNode.InsertAbove(childsUpperNeckNode);
                                parentsLowerNeckNode.Connect(childsUpperNeckNode);

                                // oberen Halsknoten des Parents und zugehörigen des childs erzeugen
                                var parentsUpperNeckNode = new NeckNode(parent,
                                    new PolarPoint(root.second.angle, parent.Surface.InterpolatedRadius(root.second.angle),
                                        parent.LocalCoordinateSystem)
                                );
                                var childsLowerNeckNode = new NeckNode(child,
                                    new PolarPoint(parentsUpperNeckNode.Coordinates, child.LocalCoordinateSystem)
                                );
                                root.second.node.InsertAbove(parentsUpperNeckNode);
                                var childsLowerSurfaceNode =
                                    child.Surface.GetNearestNodesToAngle(childsLowerNeckNode.Coordinates.Phi).Upper;
                                childsLowerSurfaceNode.InsertBelow(childsLowerNeckNode);
                                parentsUpperNeckNode.Connect(childsLowerNeckNode);

                                // Oberfläche zwischen den Halsknoten löschen
                                foreach (var surfaceNode in new SurfaceSegment(parentsLowerNeckNode, parentsUpperNeckNode)
                                    .OfType<SurfaceNode>().ToArray())
                                {
                                    surfaceNode.Remove();
                                }

                                foreach (var surfaceNode in new SurfaceSegment(childsLowerNeckNode, childsUpperNeckNode)
                                    .OfType<SurfaceNode>().ToArray())
                                {
                                    surfaceNode.Remove();
                                }

                                // Korngrenzenknoten in der Mitte zwischen den halsknoten einfügen
                                var grainBoundaryCoordinates = parentsLowerNeckNode.Coordinates.Absolute
                                    .PointHalfWayTo(parentsUpperNeckNode.Coordinates.Absolute);
                                var parentsGrainBoundaryNode =
                                    new GrainBoundaryNode(parent, new PolarPoint(grainBoundaryCoordinates, parent.LocalCoordinateSystem));
                                var childsGrainBoundaryNode =
                                    new GrainBoundaryNode(child, new PolarPoint(grainBoundaryCoordinates, child.LocalCoordinateSystem));
                                parentsGrainBoundaryNode.Connect(childsGrainBoundaryNode);
                                parentsLowerNeckNode.InsertAbove(parentsGrainBoundaryNode);
                                childsLowerNeckNode.InsertAbove(childsGrainBoundaryNode);
                                parentsLowerNeckNode.RemeshNeighborhood(session);
                                parentsUpperNeckNode.RemeshNeighborhood(session);
                                childsLowerNeckNode.RemeshNeighborhood(session);
                                childsUpperNeckNode.RemeshNeighborhood(session);
                            }

                            break;
                        }

                        var minDifference = differences.Min(x => x.diff);
                        if (minDifference > distanceStep * 10)
                            child.CenterCoordinates.R -= minDifference / 2;
                        else
                            child.CenterCoordinates.R -= distanceStep;
                    }

                    // Liefert die paarweisen Winkel, bei denen die Distanz der Oberflächen 0 wird. Paare schließen immer Bereiche negativer Distanzen ein
                    IEnumerable<((double angle, Node node) first, (double angle, Node node) second)> GetDistanceRoots(IList<(
                        Node node, double diff)> differences)
                    {
                        var lastDifference = differences.First();
                        (Angle angle, Node node)? lastRoot = null;
                        (Angle angle, Node node)? setbackRoot = null;
                        foreach (var difference in differences.Skip(1).Append(differences.First()))
                        {
                            if (lastDifference.diff * difference.diff < 0)
                            {
                                var rootAngle = -lastDifference.diff / (difference.diff - lastDifference.diff) *
                                                (difference.node.Coordinates.Phi - lastDifference.node.Coordinates.Phi).Reduce(Angle.ReductionDomain
                                                    .WithNegative) +
                                                lastDifference.node.Coordinates.Phi;

                                if (lastDifference.diff > 0) // von positiv nach negativ
                                {
                                    lastRoot = (rootAngle, node: difference.node); // speichere Nullstelle
                                }
                                // von negativ nach positiv
                                else
                                {
                                    if (lastRoot.HasValue) // lastRoot enthält einen Wert
                                    {
                                        yield return
                                            (lastRoot.Value,
                                                (rootAngle, node: lastDifference.node)); // gib beide Nullstellen als Paar zurück
                                        lastRoot = null; // leere Speicher
                                    }
                                    else // zuerst eine negativ nach positiv NS gefunden
                                        setbackRoot = (rootAngle, node: lastDifference.node); // speichere für letztes Paar}
                                }
                            }

                            lastDifference = difference;
                        }

                        if (lastRoot.HasValue && setbackRoot.HasValue) // wenn zuerst eine negativ nach positiv NS gefunden
                            yield return (lastRoot.Value, setbackRoot.Value); // gib letztes Paar zurück
                    }
                }
            }
        }
    }
}