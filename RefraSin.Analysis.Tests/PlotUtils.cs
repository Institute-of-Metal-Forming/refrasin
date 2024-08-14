using RefraSin.Coordinates;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using ScottPlot;

namespace RefraSin.Analysis.Tests;

public static class PlotUtils
{
    public static void PlotParticle(this Plot plot, IParticle<IParticleNode> particle)
    {
        plot.Add.Scatter(
            particle
                .Nodes.Append(particle.Nodes[0])
                .Select(n => new ScottPlot.Coordinates(
                    n.Coordinates.Absolute.X,
                    n.Coordinates.Absolute.Y
                ))
                .ToArray()
        );
    }

    public static void PlotPoints(this Plot plot, IEnumerable<IPoint> points)
    {
        var pointsArray = points as IPoint[] ?? points.ToArray();
        plot.Add.Scatter(
            pointsArray
                .Append(pointsArray[0])
                .Select(p => new ScottPlot.Coordinates(p.Absolute.X, p.Absolute.Y))
                .ToArray()
        );
    }
}
