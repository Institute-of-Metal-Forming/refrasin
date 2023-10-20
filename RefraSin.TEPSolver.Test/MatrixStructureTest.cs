using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.TEPSolver.Step;
using ScottPlot;

namespace RefraSin.TEPSolver.Test;

public class MatrixStructureTest
{
    private IParticleSpec _particleSpec;
    private StepVectorMap _map;

    [SetUp]
    public void Setup()
    {
        _particleSpec = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, Guid.Empty).GetParticleSpec();

        _map = new StepVectorMap(new[] { _particleSpec }, _particleSpec.NodeSpecs);
    }

    [Test]
    public void PlotMatrixStructure()
    {
        var matrix = Matrix<double>.Build.SparseOfRows(YieldEquations());

        var plt = new Plot();
        plt.XAxes.ForEach(x => x.IsVisible = false);
        plt.YAxes.ForEach(x => x.IsVisible = false);
        plt.Margins(0, 0);

        plt.Add.Heatmap(matrix.ToArray());

        plt.SavePng(Path.GetTempFileName().Replace(".tmp", ".png"), _map.TotalUnknownsCount, _map.TotalUnknownsCount);
    }

    internal IEnumerable<Vector<double>> YieldEquations()
    {
        foreach (var (i, node) in _particleSpec.NodeSpecs.Index())
        {
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(GlobalUnknown.Lambda1), 1),
                (_map.GetIndex(node.Id, NodeUnknown.Lambda2), 1),
            });
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(node.Id, NodeUnknown.FluxToUpper), 1),
                (_map.GetIndex(GlobalUnknown.Lambda1), 1),
                (_map.GetIndex(node.Id, NodeUnknown.Lambda2), 1),
                (_map.GetIndex(_particleSpec[i + 1].Id, NodeUnknown.Lambda2), 1),
            });
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(node.Id, NodeUnknown.NormalDisplacement), 1),
                (_map.GetIndex(node.Id, NodeUnknown.FluxToUpper), 1),
                (_map.GetIndex(_particleSpec[i - 1].Id, NodeUnknown.FluxToUpper), 1),
            });
        }

        yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, _particleSpec.NodeSpecs.SelectMany((n, i) => new (int, double)[]
        {
            (_map.GetIndex(n.Id, NodeUnknown.NormalDisplacement), 1),
            (_map.GetIndex(n.Id, NodeUnknown.FluxToUpper), 2),
            (_map.GetIndex(_particleSpec[i - 1].Id, NodeUnknown.FluxToUpper), 2)
        }));
    }
}