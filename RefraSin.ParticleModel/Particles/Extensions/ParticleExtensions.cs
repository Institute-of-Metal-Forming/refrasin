using MoreLinq;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using static RefraSin.Coordinates.Constants;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Particles.Extensions;

public static class ParticleExtensions
{
    public static bool MayHasContactToByRectangularApproximation(
        this IParticleMeasures self,
        IParticleMeasures other
    ) =>
        DoOverlap(self.MinX, self.MaxX, other.MinX, other.MaxX)
        && DoOverlap(self.MinY, self.MaxY, other.MinY, other.MaxY);

    private static bool DoOverlap(double selfMin, double selfMax, double otherMin, double otherMax)
    {
        if (selfMax >= otherMin)
        {
            return selfMin <= otherMax;
        }

        if (otherMax >= selfMin)
        {
            return otherMin <= selfMax;
        }

        return false;
    }

    public static bool ContainsPoint(
        this IParticle<IParticleNode> self,
        IPoint point,
        double precision = 1e-8
    )
    {
        var nodesPolarCoordinates = new PolarPoint(point, self);

        var upperNeighbor = self.Nodes.NextUpperNodeFrom(nodesPolarCoordinates.Phi);
        var lowerNeighbor = upperNeighbor.Lower;

        if (
            nodesPolarCoordinates.R > upperNeighbor.Coordinates.R
            && nodesPolarCoordinates.R > lowerNeighbor.Coordinates.R
        )
            return false;

        var radiusAtAngle =
            SinLaw.A(
                upperNeighbor.Coordinates.R,
                upperNeighbor.SurfaceRadiusAngle.ToLower,
                upperNeighbor.SurfaceRadiusAngle.ToLower
                    + nodesPolarCoordinates.AngleTo(upperNeighbor.Coordinates)
            ) + precision;

        return radiusAtAngle >= nodesPolarCoordinates.R;
    }

    public static IParticleMeasures ToMeasures(this IParticle<IParticleNode> self) =>
        self as IParticleMeasures ?? new ParticleMeasures(self);

    public static bool HasContactTo(
        this IParticle<IParticleNode> self,
        IParticle<IParticleNode> other
    )
    {
        var selfMeasures = self.ToMeasures();
        var otherMeasures = other.ToMeasures();
        if (!MayHasContactToByRectangularApproximation(selfMeasures, otherMeasures))
            return false;

        var othersCenterPolar = new PolarPoint(other.Coordinates, self);
        var sightAngle = SinLaw.Alpha(otherMeasures.MaxRadius, othersCenterPolar.R, HalfOfPi);
        IReadOnlyList<IParticleNode> possibleNodes;

        if (!double.IsNaN(sightAngle))
        {
            var minAngle = othersCenterPolar.Phi - sightAngle;
            var maxAngle = othersCenterPolar.Phi + sightAngle;

            var lowestPossibleNode = self.Nodes.NextLowerNodeFrom(minAngle);
            var highestPossibleNode = self.Nodes.NextUpperNodeFrom(maxAngle);
            possibleNodes = self.Nodes[lowestPossibleNode.Index, highestPossibleNode.Index];
        }
        else
            possibleNodes = self.Nodes;

        return self.ContainsPoint(other.Nodes[0].Coordinates) // to catch edge case when other is fully contained in self
            || possibleNodes.Any(n => other.ContainsPoint(n.Coordinates));
    }

    public static IEnumerable<IPolarPoint> IntersectionPointsTo(
        this IParticle<IParticleNode> self,
        IParticle<IParticleNode> other
    )
    {
        var othersCenterPolar = new PolarPoint(other.Coordinates, self);
        var sightAngle = SinLaw.Alpha(other.ToMeasures().MaxRadius, othersCenterPolar.R, HalfOfPi);
        IReadOnlyList<IParticleNode> possibleNodes;

        if (!double.IsNaN(sightAngle))
        {
            var minAngle = othersCenterPolar.Phi - sightAngle;
            var maxAngle = othersCenterPolar.Phi + sightAngle;

            var lowestPossibleNode = self.Nodes.NextLowerNodeFrom(minAngle);
            var highestPossibleNode = self.Nodes.NextUpperNodeFrom(maxAngle);
            possibleNodes = self.Nodes[lowestPossibleNode.Index, highestPossibleNode.Index];
        }
        else
            possibleNodes = self.Nodes;

        IPolarPoint? firstPoint = null;
        bool yieldFirstAtEnd = false;
        bool currentlyIn = other.ContainsPoint(possibleNodes[0].Coordinates);

        foreach (var node in possibleNodes.Skip(1))
        {
            var nodeIn = other.ContainsPoint(node.Coordinates);

            if (nodeIn == currentlyIn)
                continue; // no changes

            if (currentlyIn) // exiting
            {
                currentlyIn = false;
                var intersection = CalculateIntersectionTo(self, other, node);

                if (firstPoint is not null)
                {
                    yield return intersection;
                }
                else
                {
                    firstPoint = intersection;
                    yieldFirstAtEnd = true;
                }
            }
            else // entering
            {
                currentlyIn = true;
                var intersection = CalculateIntersectionTo(self, other, node);

                firstPoint ??= intersection;
                yield return intersection;
            }
        }

        if (yieldFirstAtEnd)
            yield return firstPoint!;
    }

    private static IPolarPoint CalculateIntersectionTo(
        IParticle<IParticleNode> self,
        IParticle<IParticleNode> other,
        IParticleNode selfUpperNode
    )
    {
        var selfLine = new AbsoluteLine(selfUpperNode.Lower.Coordinates, selfUpperNode.Coordinates);
        var otherUpperNode = other.Nodes.NextUpperNodeFrom(
            new PolarPoint(selfUpperNode.Coordinates, other).Phi
        );
        var otherLine = new AbsoluteLine(
            otherUpperNode.Lower.Coordinates,
            otherUpperNode.Coordinates
        );

        var intersection = selfLine.IntersectionTo(otherLine);
        return new PolarPoint(intersection, self);
    }

    public static (
        Particle<ParticleNode> self,
        Particle<ParticleNode> other
    ) CreateGrainBoundariesAtIntersections(
        this IParticle<IParticleNode> self,
        IParticle<IParticleNode> other
    )
    {
        var intersections = self.IntersectionPointsTo(other).ToArray();

        var selfNeckPairs = intersections
            .TakeEvery(2)
            .Zip(intersections.Skip(1).TakeEvery(2))
            .ToArray();
        var otherNeckPairs = selfNeckPairs
            .Select(p =>
                (
                    (IPolarPoint)new PolarPoint(p.Second, other),
                    (IPolarPoint)new PolarPoint(p.First, other)
                )
            )
            .ToArray();

        var newSelf = new Particle<ParticleNode>(
            self.Id,
            self.Coordinates,
            self.RotationAngle,
            self.MaterialId,
            particle => NodeFactory(self, particle, selfNeckPairs)
        );
        var newOther = new Particle<ParticleNode>(
            other.Id,
            other.Coordinates,
            other.RotationAngle,
            other.MaterialId,
            particle => NodeFactory(other, particle, otherNeckPairs)
        );

        return (newSelf, newOther);

        IEnumerable<ParticleNode> NodeFactory(
            IParticle<IParticleNode> template,
            IParticle<ParticleNode> particle,
            IEnumerable<(IPolarPoint Lower, IPolarPoint Upper)> neckPairs
        )
        {
            var nodes = (IParticleSurface<ParticleNode>)new ParticleSurface<ParticleNode>(
                template.Nodes.Select(n => new ParticleNode(n, particle))
            );

            foreach (var neckPair in neckPairs)
            {
                var lowerToRemove = nodes.NextUpperNodeFrom(neckPair.Lower.Phi);
                var upperToRemove = nodes.NextLowerNodeFrom(neckPair.Upper.Phi);
                var lowerRemaining = nodes.LowerNeighborOf(lowerToRemove); 
                nodes.Remove(lowerToRemove, upperToRemove);

                var lowerNeckPoint = new PolarPoint(neckPair.Lower, particle);
                var upperNeckPoint = new PolarPoint(neckPair.Upper, particle);
                var grainBoundaryPoint = lowerNeckPoint.Centroid(upperNeckPoint);

                nodes.InsertAbove(
                    lowerRemaining.Id,
                    [
                        new ParticleNode(Guid.NewGuid(), particle, lowerNeckPoint, Neck),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            grainBoundaryPoint,
                            GrainBoundary
                        ),
                        new ParticleNode(Guid.NewGuid(), particle, upperNeckPoint, Neck)
                    ]
                );
            }

            return nodes;
        }
    }
}
