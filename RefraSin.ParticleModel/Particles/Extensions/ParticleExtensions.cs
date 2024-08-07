using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;

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
        IParticle<IParticleNode> other,
        bool checkSymmetrically = true
    )
    {
        if (!MayHasContactToByRectangularApproximation(self.ToMeasures(), other.ToMeasures()))
            return false;

        var othersCenterPolar = new PolarPoint(other.Coordinates, self);
        var minAngle = othersCenterPolar.Phi - Angle.Right;
        var maxAngle = othersCenterPolar.Phi + Angle.Right;

        var possibleNodes = self.Nodes[
            self.Nodes.NextLowerNodeFrom(minAngle).Index,
            self.Nodes.NextUpperNodeFrom(maxAngle).Index
        ];

        return possibleNodes.Any(n => other.ContainsPoint(n.Coordinates))
            || (checkSymmetrically && other.HasContactTo(self, false)); // fallback reverse calculation for edge cases where one particle contains another
    }
}
