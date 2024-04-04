using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.EquationSystem;
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
        var duration = 1e2;

        _particle = new ShapeFunctionParticleFactory(
            100e-6,
            0.1,
            5,
            0.1,
            Guid.NewGuid()
        ).GetParticle();
        _solutionStorage = new InMemorySolutionStorage();

        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile(Path.Combine(_tempDir, "test.log"));
        });

        _solver = new Solver(
            _solutionStorage,
            loggerFactory,
            SolverRoutines.Default,
            new SolverOptions
            {
                InitialTimeStepWidth = 1,
                MinTimeStepWidth = 0.1,
                TimeStepAdaptationFactor = 1.5,
            }
        );

        _material = new Material(
            _particle.MaterialId,
            "Al2O3",
            1.65e-10,
            0,
            1e-4,
            0.9,
            1.8e3,
            101.96e-3
        );

        _materialInterface = new MaterialInterface(_material.Id, _material.Id, 0.5, 1.65e-10, 0);

        _initialState = new SystemState(
            0,
            new[] { _particle },
            new[] { _material },
            new[] { _materialInterface }
        );

        _sinteringProcess = new SinteringStep(duration, 2073, _solver);
    }

    private IParticle _particle;
    private Solver _solver;
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

        var particleBlocks = initialState
            .Particles.Select(p => Jacobian.ParticleBlock(session, p, guess))
            .ToArray();
        var functionalBlock = Jacobian.BorderBlock(session, initialState, guess);
        var size = particleBlocks.Length + 1;

        var array = new Matrix<double>[size, size];

        for (int i = 0; i < particleBlocks.Length; i++)
        {
            for (int j = 0; j < particleBlocks.Length; j++)
            {
                array[i, j] = Matrix<double>.Build.Sparse(
                    particleBlocks[i].RowCount,
                    particleBlocks[j].ColumnCount
                );
            }

            array[i, particleBlocks.Length] = Matrix<double>.Build.Sparse(
                particleBlocks[i].RowCount,
                functionalBlock.ColumnCount
            );

            array[i, i] = particleBlocks[i].PointwiseSign();
        }

        for (int j = 0; j < particleBlocks.Length; j++)
        {
            array[particleBlocks.Length, j] = Matrix<double>.Build.Sparse(
                functionalBlock.RowCount,
                particleBlocks[j].ColumnCount
            );
        }

        array[particleBlocks.Length, particleBlocks.Length] = functionalBlock.PointwiseSign();

        var matrix = Matrix<double>.Build.SparseOfMatrixArray(array);

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
        var zero = Lagrangian.EvaluateAt(session, state, guess);

        for (int i = 0; i < guess.Count; i++)
        {
            var step = guess.Copy();
            step[i] += 1e-3;
            var current = Lagrangian.EvaluateAt(session, state, step);

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

            plt.SavePng(Path.Combine(dir, $"{i}.png"), 3000, 3000);
        }
    }

    private void PlotDisplacements()
    {
        var dir = Path.Combine(_tempDir, "nd");
        Directory.CreateDirectory(dir);

        foreach (var (i, step) in _solutionStorage.Steps.Index())
        {
            var plt = new Plot();

            var sinteringStep = (ISinteringStateStateTransition)step;

            var coordinates = sinteringStep
                .ParticleTimeSteps[0]
                .NodeTimeSteps.Values.Select(
                    (n, j) => new ScottPlot.Coordinates(j, n.NormalDisplacement)
                )
                .ToArray();
            plt.Add.Scatter(coordinates);

            plt.Add.Line(0, 0, coordinates.Length, 0);

            plt.Title(
                $"t = {step.InputState.Time.ToString(CultureInfo.InvariantCulture)} - {step.OutputState.Time.ToString(CultureInfo.InvariantCulture)}"
            );

            plt.SavePng(Path.Combine(dir, $"{i}.png"), 600, 400);
        }
    }

    private void PlotTimeSteps()
    {
        var plt = new Plot();

        var steps = _solutionStorage
            .Steps.Select(s => new ScottPlot.Coordinates(
                s.InputState.Time,
                ((ISinteringStateStateTransition)s).TimeStepWidth
            ))
            .ToArray();
        plt.Add.Scatter(steps);

        var meanStepWidth = steps.Select(s => s.Y).Mean();
        plt.Add.Line(0, meanStepWidth, _sinteringProcess.Duration, meanStepWidth);

        plt.SavePng(Path.Combine(_tempDir, "timeSteps.png"), 600, 400);
    }

    private void PlotParticleCenter()
    {
        var plt = new Plot();

        plt.Add.Scatter(
            _solutionStorage
                .States.Select(s => new ScottPlot.Coordinates(
                    s.Time,
                    s.Particles[0].CenterCoordinates.Absolute.X
                ))
                .ToArray()
        );
        plt.Add.Scatter(
            _solutionStorage
                .States.Select(s => new ScottPlot.Coordinates(
                    s.Time,
                    s.Particles[0].CenterCoordinates.Absolute.Y
                ))
                .ToArray()
        );

        plt.SavePng(Path.Combine(_tempDir, "particleCenter.png"), 600, 400);
    }
}
