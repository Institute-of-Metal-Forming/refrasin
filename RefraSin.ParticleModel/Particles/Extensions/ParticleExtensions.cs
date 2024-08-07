using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using static RefraSin.Coordinates.Constants;

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
}
