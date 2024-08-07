using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles.Extensions;

public static class ParticleExtensions
{
    public static bool MayHaveContactToByRectangularApproximation(
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

    public static bool PointIsInParticle(this IParticle<IParticleNode> self, IPoint point)
    {
        var nodesPolarCoordinates = new PolarPoint(point, self);

        var upperNeighbor = self.Nodes.NextUpperNodeFrom(nodesPolarCoordinates.Phi);
        var lowerNeighbor = upperNeighbor.Lower;

        if (
            nodesPolarCoordinates.R > upperNeighbor.Coordinates.R
            && nodesPolarCoordinates.R > lowerNeighbor.Coordinates.R
        )
            return false;

        var radiusAtAngle = SinLaw.A(
            upperNeighbor.Coordinates.R,
            upperNeighbor.SurfaceRadiusAngle.ToLower,
            upperNeighbor.SurfaceRadiusAngle.ToLower
                + nodesPolarCoordinates.AngleTo(upperNeighbor.Coordinates)
        );

        return radiusAtAngle > nodesPolarCoordinates.R;
    }
}
