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

public static class ParticleIntersectionExtensions
{
    public static bool MayIntersectWithByRectangularApproximation(
        this IParticleMeasures self,
        IParticleMeasures other
    ) =>
        RangesOverlap(self.MinX, self.MaxX, other.MinX, other.MaxX)
        && RangesOverlap(self.MinY, self.MaxY, other.MinY, other.MaxY);

    private static bool RangesOverlap(
        double selfMin,
        double selfMax,
        double otherMin,
        double otherMax
    )
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

    public static IParticleMeasures ToMeasures<TParticle, TNode>(this TParticle self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode =>
        self as IParticleMeasures ?? new ParticleMeasures((IParticle<IParticleNode>)self);

    public static bool IntersectsWith(
        this IParticle<IParticleNode> self,
        IParticle<IParticleNode> other,
        double precision = 1e-8
    )
    {
        var selfMeasures = self.ToMeasures();
        var otherMeasures = other.ToMeasures();
        if (!MayIntersectWithByRectangularApproximation(selfMeasures, otherMeasures))
            return false;

        var possibleNodes = self.Nodes;

        return self.ContainsPoint(other.Nodes[0].Coordinates, precision) // to catch edge case when other is fully contained in self
            || possibleNodes.Any(n => other.ContainsPoint(n.Coordinates, precision));
    }

    public static IEnumerable<IPolarPoint> IntersectionPointsTo(
        this IParticle<IParticleNode> self,
        IParticle<IParticleNode> other
    )
    {
        IPolarPoint? firstPoint = null;
        bool yieldFirstAtEnd = false;
        bool currentlyIn = other.ContainsPoint(self.Nodes[0].Coordinates);

        foreach (var node in self.Nodes.Skip(1))
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
        var otherUpperNode = other
            .Nodes.NextUpperNodeFrom(new PolarPoint(selfUpperNode.Coordinates, other).Phi)
            .Lower.Lower.Lower;
        while (true)
        {
            var otherLine = new AbsoluteLine(
                otherUpperNode.Lower.Coordinates,
                otherUpperNode.Coordinates
            );

            try
            {
                var intersection = selfLine.IntersectionTo(otherLine);
                var intersectionSelfsPolar = new PolarPoint(intersection, self);
                var intersectionOthersPolar = new PolarPoint(intersection, other);
                if (
                    intersectionSelfsPolar.Phi < selfUpperNode.Coordinates.Phi
                    && intersectionOthersPolar.Phi < otherUpperNode.Coordinates.Phi
                    && intersectionOthersPolar.Phi > otherUpperNode.Lower.Coordinates.Phi
                )
                    return intersectionSelfsPolar;
            }
            catch (InvalidOperationException) { }

            otherUpperNode = otherUpperNode.Upper;
        }
    }

    public static (
        IParticle<IParticleNode> self,
        IParticle<IParticleNode> other
    ) CreateGrainBoundariesAtIntersections(
        this IParticle<IParticleNode> self,
        IParticle<IParticleNode> other
    )
    {
        var mutableSelf = new MutableParticle<IParticleNode>(
            self,
            (n, p) => new ParticleNode(n, p)
        );
        var mutableOther = new MutableParticle<IParticleNode>(
            other,
            (n, p) => new ParticleNode(n, p)
        );

        mutableSelf.CreateGrainBoundariesAtIntersections(
            mutableOther,
            (point, particle) => new ParticleNode(Guid.NewGuid(), particle, point, Neck),
            (point, particle) => new ParticleNode(Guid.NewGuid(), particle, point, GrainBoundary)
        );

        return (mutableSelf, mutableOther);
    }

    public static void CreateGrainBoundariesAtIntersections<TNode>(
        this IMutableParticle<TNode> self,
        IMutableParticle<TNode> other,
        Func<IPolarPoint, IMutableParticle<TNode>, TNode> neckNodeConstructor,
        Func<IPolarPoint, IMutableParticle<TNode>, TNode> grainBoundaryNodeConstructor
    )
        where TNode : IParticleNode
    {
        var intersections = ((IParticle<IParticleNode>)self)
            .IntersectionPointsTo((IParticle<IParticleNode>)other)
            .ToArray();

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

        MutateNodes(self, selfNeckPairs);
        MutateNodes(other, otherNeckPairs);

        void MutateNodes(
            IMutableParticle<TNode> target,
            IEnumerable<(IPolarPoint Lower, IPolarPoint Upper)> neckPairs
        )
        {
            foreach (var neckPair in neckPairs)
            {
                var lowerToRemove = target.Surface.NextUpperNodeFrom(neckPair.Lower.Phi);
                var upperToRemove = target.Surface.NextLowerNodeFrom(neckPair.Upper.Phi);
                var lowerRemaining = target.Surface.LowerNeighborOf(lowerToRemove);
                target.Surface.Remove(lowerToRemove, upperToRemove);

                var lowerNeckPoint = new PolarPoint(neckPair.Lower, target);
                var upperNeckPoint = new PolarPoint(neckPair.Upper, target);
                var grainBoundaryPoint = lowerNeckPoint.Centroid(upperNeckPoint);

                target.Surface.InsertAbove(
                    lowerRemaining.Id,
                    [
                        neckNodeConstructor(lowerNeckPoint, target),
                        grainBoundaryNodeConstructor(grainBoundaryPoint, target),
                        neckNodeConstructor(upperNeckPoint, target),
                    ]
                );
            }
        }
    }
}
