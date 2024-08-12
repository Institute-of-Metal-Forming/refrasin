using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using ScottPlot;

namespace RefraSin.ParticleModel.Test;

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
}
