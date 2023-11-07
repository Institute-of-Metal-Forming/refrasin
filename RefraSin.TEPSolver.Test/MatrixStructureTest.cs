using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.TEPSolver.StepVectors;
using ScottPlot;

namespace RefraSin.TEPSolver.Test;

public class MatrixStructureTest
{
    private StepVectorMap _map;
    private IParticleSpec[] _particles;
    private INodeSpec[] _nodes;

    [SetUp]
    public void Setup()
    {
        var fac = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, Guid.Empty) { NodeCount = 20 };

        _particles = Enumerable.Range(0, 3).Select(_ => fac.GetParticleSpec()).ToArray();
        _nodes = _particles.SelectMany(p => p.NodeSpecs).ToArray();

        _map = new StepVectorMap(_particles, _nodes);
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
        yield return Vector<double>.Build.SparseOfIndexed(
            _map.TotalUnknownsCount,
            _particles.SelectMany(
                p => p.NodeSpecs.SelectMany(
                    (n, i) =>
                        new (int, double)[]
                        {
                            (_map.GetIndex(n.Id, NodeUnknown.NormalDisplacement), 1),
                            (_map.GetIndex(n.Id, NodeUnknown.FluxToUpper), 2),
                            (_map.GetIndex(p[i - 1].Id, NodeUnknown.FluxToUpper), 2)
                        }
                )
            )
        );
        // fix root particle to origin
        var first = _particles[0];
        yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
        {
            (_map.GetIndex(first.Id, ParticleUnknown.RadialDisplacement), 1),
        });
        yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
        {
            (_map.GetIndex(first.Id, ParticleUnknown.AngleDisplacement), 1),
        });
        yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
        {
            (_map.GetIndex(first.Id, ParticleUnknown.RotationDisplacement), 1),
        });

        // yield particle displacement equations
        foreach (var particle in _particles.Skip(1))
        {
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(particle.Id, ParticleUnknown.RadialDisplacement), 1),
            });
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(particle.Id, ParticleUnknown.AngleDisplacement), 1),
            });
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(particle.Id, ParticleUnknown.RotationDisplacement), 1),
            });
        }

        foreach (var particleSpec in _particles)
        foreach (var (i, node) in particleSpec.NodeSpecs.Index())
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
                (_map.GetIndex(particleSpec[i + 1].Id, NodeUnknown.Lambda2), 1),
            });
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(node.Id, NodeUnknown.NormalDisplacement), 1),
                (_map.GetIndex(node.Id, NodeUnknown.FluxToUpper), 1),
                (_map.GetIndex(particleSpec[i - 1].Id, NodeUnknown.FluxToUpper), 1),
            });
        }
    }
}