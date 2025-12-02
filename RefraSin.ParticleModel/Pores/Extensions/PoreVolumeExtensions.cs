using MoreLinq;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Pores.Extensions;

public static class PoreVolumeExtensions
{
    public static double Volume<TPore, TNode>(this TPore pore)
        where TPore : IPore<TNode>
        where TNode : INode
    {
        var xValues = pore.Nodes.Select(n => n.Coordinates.Absolute.X).ToArray();
        var yValues = pore.Nodes.Select(n => n.Coordinates.Absolute.Y).ToArray();
        var iMax = pore.Nodes.Count - 1;

        var volume = (yValues[iMax] + yValues[0]) * (xValues[iMax] - xValues[0]);

        for (int i = 0; i < iMax; i++)
        {
            volume += (yValues[i] + yValues[i + 1]) * (xValues[i] - xValues[i + 1]);
        }

        return volume / 2;
    }

    public static double VolumeDifferential<TPore, TNode>(
        this TPore pore,
        IEnumerable<double> xDifferentials,
        IEnumerable<double> yDifferentials
    )
        where TPore : IPore<TNode>
        where TNode : INode
    {
        var xValues = pore.Nodes.Select(n => n.Coordinates.Absolute.X).ToArray();
        var yValues = pore.Nodes.Select(n => n.Coordinates.Absolute.Y).ToArray();
        var n = pore.Nodes.Count;

        var volumeDifferential = 0.0;

        foreach (var (i, dx) in xDifferentials.Index())
        {
            volumeDifferential +=
                (yValues[ReduceIndex(i + 1, n)] - yValues[ReduceIndex(i - 1, n)]) * dx;
        }

        foreach (var (i, dy) in yDifferentials.Index())
        {
            volumeDifferential +=
                (xValues[ReduceIndex(i - 1, n)] - xValues[ReduceIndex(i + 1, n)]) * dy;
        }

        return volumeDifferential / 2;
    }

    public static IEnumerable<(double x, double y)> VolumeDifferentials<TPore, TNode>(
        this TPore pore
    )
        where TPore : IPore<TNode>
        where TNode : INode
    {
        var xValues = pore.Nodes.Select(n => n.Coordinates.Absolute.X).ToArray();
        var yValues = pore.Nodes.Select(n => n.Coordinates.Absolute.Y).ToArray();
        var n = pore.Nodes.Count;

        for (int i = 0; i < n; i++)
        {
            yield return (
                (yValues[ReduceIndex(i + 1, n)] - yValues[ReduceIndex(i - 1, n)]) / 2,
                (xValues[ReduceIndex(i - 1, n)] - xValues[ReduceIndex(i + 1, n)]) / 2
            );
        }
    }

    private static int ReduceIndex(int i, int n)
    {
        while (i >= n)
            i -= n;
        while (i < 0)
            i += n;
        return i;
    }
}
