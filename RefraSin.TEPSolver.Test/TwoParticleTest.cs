using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;
using Plotly.NET;
using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using ScottPlot;
using static MoreLinq.Extensions.IndexExtension;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
[TestFixtureSource(nameof(GenerateParticles))]
public class TwoParticleTest
{
    public static IEnumerable<(
        Angle firstRotation,
        IPoint secondCenter,
        Angle secondRotation,
        double secondSize
    )> YieldParticleProperties()
    {
        yield return (0, new AbsolutePoint(300e-6, 0), Angle.Straight, 100e-6);
        yield return (0, new AbsolutePoint(500e-6, 0), Angle.Straight, 200e-6);
        yield return (
            Angle.HalfRight,
            new AbsolutePoint(300e-6, 0),
            Angle.Straight - Angle.HalfRight,
            100e-6
        );
        yield return (
            Angle.HalfRight,
            new AbsolutePoint(400e-6, 0),
            Angle.Straight - Angle.HalfRight,
            200e-6
        );
        yield return (Angle.Straight, new AbsolutePoint(300e-6, 0), 0, 100e-6);
        yield return (
            -Angle.HalfRight,
            new AbsolutePoint(200e-6, -100e-6),
            Angle.Straight - Angle.HalfRight,
            100e-6
        );
    }

    public static IEnumerable<TestFixtureData> GenerateParticles()
    {
        var nodeCountPerParticle = 100;

        foreach (var props in YieldParticleProperties())
        {
            var particle1 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, Guid.NewGuid())
            {
                NodeCount = nodeCountPerParticle,
                RotationAngle = props.firstRotation
            }.GetParticle();
            var particle2 = new ShapeFunctionParticleFactory(
                props.secondSize,
                0.2,
                5,
                0.2,
                particle1.MaterialId
            )
            {
                NodeCount = nodeCountPerParticle,
                RotationAngle = props.secondRotation,
                CenterCoordinates = props.secondCenter.Absolute
            }.GetParticle();

            var looseState = new SystemState(Guid.NewGuid(), 0, [particle1, particle2]);

            var compactor = new FocalCompactionStep(
                particle1.Coordinates,
                1e-6,
                maxStepCount: 1000
            );
            var compactedState = compactor.Solve(looseState);

            ParticlePlot.PlotParticles(compactedState.Particles).Show();

            yield return new TestFixtureData(compactedState);
        }
    }

    public TwoParticleTest(SystemState initialState)
    {
        var duration = 3.6e4;

        _solutionStorage = new InMemorySolutionStorage();

        _tempDir = TempPath.CreateTempDir();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile(Path.Combine(_tempDir, "test.log"));
        });

        _solver = new SinteringSolver(_solutionStorage, loggerFactory, SolverRoutines.Default, 10);

        _initialState = initialState;

        var material = new Material(
            initialState.Particles[0].MaterialId,
            "Al2O3",
            new BulkProperties(0, 1e-4),
            new SubstanceProperties(1.8e3, 101.96e-3),
            new InterfaceProperties(1.65e-10, 0.9),
            new Dictionary<Guid, IInterfaceProperties>
            {
                { initialState.Particles[1].MaterialId, new InterfaceProperties(1.65e-10, 0.5) }
            }
        );

        _sinteringProcess = new SinteringStep(duration, 2073, _solver, new[] { material });
        _sinteringProcess.UseStorage(_solutionStorage);
    }

    private SinteringSolver _solver;
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

        var matrix = new EquationSystem.EquationSystem(initialState, guess)
            .Jacobian()
            .PointwiseSign();

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
        var zero = new EquationSystem.EquationSystem(state, guess).Lagrangian();

        for (int i = 0; i < guess.Count; i++)
        {
            var step = guess.Copy();
            step[i] += 1e-3;
            var current = new EquationSystem.EquationSystem(state, step).Lagrangian();

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
            PlotShrinkage();
            PlotNeckWidths();
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
            var plot = ParticlePlot.PlotParticles(state.Particles);
            plot.WithXAxisStyle(
                Title.init(Text: "X in m"),
                MinMax: FSharpOption<Tuple<IConvertible, IConvertible>>.Some(new(-200e-6, 700e-6))
            );
            plot.WithYAxisStyle(
                Title.init(Text: "Y in m"),
                MinMax: FSharpOption<Tuple<IConvertible, IConvertible>>.Some(new(-200e-6, 200e-6))
            );
            plot.SaveHtml(Path.Combine(dir, $"{i}.html"));
        }
    }

    private void PlotTimeSteps()
    {
        var plot = ProcessPlot.PlotTimeSteps(_solutionStorage.States);
        plot.SaveHtml(Path.Combine(_tempDir, "timeSteps.html"));
    }

    private void PlotParticleCenter()
    {
        var centers = ProcessPlot.PlotParticleCenters(_solutionStorage.States);
        var start = ParticlePlot.PlotParticles(_solutionStorage.States[0].Particles);
        var end = ParticlePlot.PlotParticles(_solutionStorage.States[^1].Particles);
        var plot = Chart.Combine([centers, start, end]);
        plot.SaveHtml(Path.Combine(_tempDir, "particleCenters.html"));
    }

    private void PlotShrinkage()
    {
        var plot = ProcessPlot.PlotShrinkagesByDistance(_solutionStorage.States);
        plot.SaveHtml(Path.Combine(_tempDir, "shrinkages.html"));
    }

    private void PlotNeckWidths()
    {
        var plot = ProcessPlot.PlotNeckWidths(_solutionStorage.States);
        plot.SaveHtml(Path.Combine(_tempDir, "necks.html"));
    }
}
