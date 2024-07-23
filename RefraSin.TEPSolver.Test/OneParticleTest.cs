using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using ScottPlot;
using static MoreLinq.Extensions.IndexExtension;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class OneParticleTest
{
    [SetUp]
    public void Setup()
    {
        var duration = 1e4;

        _particle = new ShapeFunctionParticleFactory(100e-6, 0.1, 5, 0.1, Guid.NewGuid())
        {
            NodeCount = 80
        }.GetParticle();
        _solutionStorage = new InMemorySolutionStorage();

        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile(Path.Combine(_tempDir, "test.log"));
        });

        _solver = new SinteringSolver(_solutionStorage, loggerFactory, SolverRoutines.Default);

        _material = new Material(
            _particle.MaterialId,
            "Al2O3",
            new BulkProperties(0, 1e-4),
            new SubstanceProperties(1.8e3, 101.96e-3),
            new InterfaceProperties(1.65e-10, 0.9)
        );

        _materialInterface = new MaterialInterface(
            _material.Id,
            _material.Id,
            new InterfaceProperties(1.65e-10, 0.5)
        );

        _initialState = new SystemState(Guid.NewGuid(), 0, new[] { _particle });

        _sinteringProcess = new SinteringStep(
            duration,
            2073,
            _solver,
            new[] { _material },
            new[] { _materialInterface }
        );
        _sinteringProcess.UseStorage(_solutionStorage);
    }

    private IParticle<IParticleNode> _particle;
    private SinteringSolver _solver;
    private IMaterial _material;
    private IMaterialInterface _materialInterface;
    private SystemState _initialState;
    private SinteringStep _sinteringProcess;
    private InMemorySolutionStorage _solutionStorage;
    private string _tempDir;

    [Test]
    public void PlotJacobianStructureAnalytical()
    {
        var session = new SolverSession(_solver, _initialState, _sinteringProcess);
        var initialState = session.CurrentState;
        var guess = session.Routines.StepEstimator.EstimateStep(session, initialState);
        var matrix = Jacobian.EvaluateAt(initialState, guess).PointwiseSign();

        var plt = new Plot();

        plt.Add.Heatmap(matrix.ToArray());
        plt.Layout.Frameless();
        plt.Axes.Margins(0, 0);

        plt.SavePng(Path.Combine(_tempDir, "jacobian.png"), matrix.ColumnCount, matrix.RowCount);
    }

    [Test]
    public void PlotJacobianStructureNumerical()
    {
        var session = new SolverSession(_solver, _initialState, _sinteringProcess);
        var initialState = session.CurrentState;
        var guess = session.Routines.StepEstimator.EstimateStep(session, initialState);

        var matrix = Matrix<double>
            .Build.DenseOfColumns(YieldJacobianColumns(session, initialState, guess))
            .PointwiseSign();

        var plt = new Plot();

        plt.Add.Heatmap(matrix.ToArray());
        plt.Layout.Frameless();
        plt.Axes.Margins(0, 0);

        plt.SavePng(Path.Combine(_tempDir, "jacobian.png"), matrix.ColumnCount, matrix.RowCount);
    }

    private IEnumerable<Vector<double>> YieldJacobianColumns(
        SolverSession session,
        SolutionState state,
        StepVector guess
    )
    {
        var zero = Lagrangian.EvaluateAt(state, guess);

        for (int i = 0; i < guess.Count; i++)
        {
            var step = guess.Copy();
            step[i] += 1e-3;
            var current = Lagrangian.EvaluateAt(state, step);

            yield return current - zero;
        }
    }

    [Test]
    public void TestSolution()
    {
        try
        {
            _sinteringProcess.Solve(_initialState);
        }
        finally
        {
            PlotParticles();
            PlotDisplacements();
            PlotTimeSteps();
            PlotParticleCenter();
        }
    }

    private void PlotParticles()
    {
        var dir = Path.Combine(_tempDir, "p");
        Directory.CreateDirectory(dir);

        foreach (var (i, state) in _solutionStorage.States.Index())
        {
            var plt = new Plot();
            plt.Axes.SquareUnits();

            var coordinates = state
                .Particles[0]
                .Nodes.Append(state.Particles[0].Nodes[0])
                .Select(n => new ScottPlot.Coordinates(
                    n.Coordinates.Absolute.X,
                    n.Coordinates.Absolute.Y
                ))
                .ToArray();
            plt.Add.Scatter(coordinates);

            plt.Title($"t = {state.Time.ToString(CultureInfo.InvariantCulture)}");

            plt.SavePng(Path.Combine(dir, $"{i}.png"), 1600, 900);
        }
    }

    private void PlotDisplacements()
    {
        var dir = Path.Combine(_tempDir, "nd");
        Directory.CreateDirectory(dir);

        foreach (var (i, step) in _solutionStorage.States.Index())
        {
            var plt = new Plot();

            var coordinates = step.Particles[0]
                .Nodes.Select(
                    (n, j) =>
                        n switch
                        {
                            INodeShifts shifts => new ScottPlot.Coordinates(j, shifts.Shift.Normal),
                            _ => new ScottPlot.Coordinates()
                        }
                )
                .ToArray();
            plt.Add.Scatter(coordinates);

            plt.Add.Line(0, 0, coordinates.Length, 0);

            plt.Title($"t = {step.Time.ToString(CultureInfo.InvariantCulture)}");

            plt.SavePng(Path.Combine(dir, $"{i}.png"), 600, 400);
        }
    }

    private void PlotTimeSteps()
    {
        var plt = new Plot();

        var steps = _solutionStorage
            .States.Skip(1)
            .Zip(_solutionStorage.States)
            .Select(
                (s, i) => new ScottPlot.Coordinates(i, Math.Log10(s.First.Time - s.Second.Time))
            )
            .ToArray();
        plt.Add.Scatter(steps);

        var meanStepWidth = steps.Select(s => s.Y).Mean();
        var meanLine = plt.Add.HorizontalLine(meanStepWidth);
        meanLine.Text = "mean";

        plt.SavePng(Path.Combine(_tempDir, "timeSteps.png"), 600, 400);
    }

    private void PlotParticleCenter()
    {
        var plt = new Plot();

        plt.Add.Scatter(
            _solutionStorage
                .States.Select(s => new ScottPlot.Coordinates(
                    s.Time,
                    s.Particles[0].Coordinates.Absolute.X
                ))
                .ToArray()
        );
        plt.Add.Scatter(
            _solutionStorage
                .States.Select(s => new ScottPlot.Coordinates(
                    s.Time,
                    s.Particles[0].Coordinates.Absolute.Y
                ))
                .ToArray()
        );

        plt.SavePng(Path.Combine(_tempDir, "particleCenter.png"), 600, 400);
    }
}
