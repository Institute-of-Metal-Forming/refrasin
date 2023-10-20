using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Absolute;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.Step;
using ScottPlot;

namespace RefraSin.TEPSolver.Test;

public class MatrixStructureTest
{
    private SolverSession _solverSession;
    private IParticleSpec _particleSpec;
    private StepVectorMap _map;

    [SetUp]
    public void Setup()
    {
        var material = new Material(
            Guid.NewGuid(),
            "Al2O3",
            1e-9,
            0,
            1e-4,
            1,
            1.8e3,
            101.96e-3
        );

        var materialInterface = new MaterialInterface(
            material.Id,
            material.Id,
            0.5,
            1e-9,
            0
        );

        _particleSpec = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, material.Id).GetParticleSpec();

        var solver = new Solver
        {
            LoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); }),
            Options = new SolverOptions
            {
                InitialTimeStepWidth = 0.01
            }
        };

        var process = new SinteringProcess(
            0,
            1,
            new[] { _particleSpec },
            new[] { material },
            new[] { materialInterface },
            1273
        );

        _solverSession = solver.CreateSession(process);
        _map = new StepVectorMap(_solverSession);
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
        foreach (var node in _solverSession.Nodes.Values)
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
                (_map.GetIndex(node.Upper.Id, NodeUnknown.Lambda2), 1),
            });
            yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, new (int, double)[]
            {
                (_map.GetIndex(node.Id, NodeUnknown.NormalDisplacement), 1),
                (_map.GetIndex(node.Id, NodeUnknown.FluxToUpper), 1),
                (_map.GetIndex(node.Lower.Id, NodeUnknown.FluxToUpper), 1),
            });
        }

        yield return Vector<double>.Build.SparseOfIndexed(_map.TotalUnknownsCount, _solverSession.Nodes.Values.SelectMany(n => new (int, double)[]
        {
            (_map.GetIndex(n.Id, NodeUnknown.NormalDisplacement), 1),
            (_map.GetIndex(n.Id, NodeUnknown.FluxToUpper), 2),
            (_map.GetIndex(n.Lower.Id, NodeUnknown.FluxToUpper), 2)
        }));
    }
}