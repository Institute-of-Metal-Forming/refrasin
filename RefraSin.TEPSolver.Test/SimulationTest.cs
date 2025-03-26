using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;
using Plotly.NET;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using static MoreLinq.Extensions.IndexExtension;

namespace RefraSin.TEPSolver.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class SimulationTest
{
    public SimulationTest(ISystemState<IParticle<IParticleNode>, IParticleNode> initialState)
    {
        _initialState = initialState;

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile(Path.Combine(_tempDir, "test.log"));
        });

        var solver = new SinteringSolver(loggerFactory, SolverRoutines.Default, 10);

        _sinteringProcess = new SinteringStep(Conditions, solver, [Material]);
        _sinteringProcess.UseStorage(_solutionStorage);
        _sinteringProcess.UseStorage(
            new ParquetStorage.ParquetStorage(Path.Combine(_tempDir, "results.parquet"))
        );
    }

    public static IEnumerable<TestFixtureData> GetTestFixtureData() =>
        InitialStates.Generate().Select(s => new TestFixtureData(s.state) { TestName = s.label });

    private static readonly ISinteringConditions Conditions = new SinteringConditions(2073, 3.6e4);
    private readonly ISystemState<IParticle<IParticleNode>, IParticleNode> _initialState;
    private readonly SinteringStep _sinteringProcess;
    private readonly InMemorySolutionStorage _solutionStorage = new();

    private static readonly IMaterial Material = new Material(
        InitialStates.MaterialId,
        "Al2O3",
        new BulkProperties(0, 1e-4),
        new SubstanceProperties(1.8e3, 101.96e-3),
        new InterfaceProperties(1.65e-10, 0.9),
        new Dictionary<Guid, IInterfaceProperties>
        {
            { InitialStates.MaterialId, new InterfaceProperties(1.65e-10, 0.5) },
        }
    );

    private readonly string _tempDir = TempPath.CreateTempDir();

    [Test]
    public void TestSolution()
    {
        Exception? exception = null;

        try
        {
            var finalState = _sinteringProcess.Solve(_initialState);
            ParticlePlot
                .PlotParticles<IParticle<IParticleNode>, IParticleNode>(finalState.Particles)
                .SaveHtml(Path.Combine(_tempDir, "final.html"));
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            PlotParticles();
            PlotShrinkage();
            PlotNeckWidths();
            PlotTimeSteps();
            PlotParticleCenter();
        }

        if (exception is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }

    private void PlotParticles()
    {
        var dir = Path.Combine(_tempDir, "p");
        Directory.CreateDirectory(dir);

        foreach (var (i, state) in _solutionStorage.States.Index())
        {
            var plot = ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
                state.Particles
            );
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
        if (_solutionStorage.States.Count == 0)
            return;

        var plot = ProcessPlot.PlotTimeSteps(_solutionStorage.States);
        plot.SaveHtml(Path.Combine(_tempDir, "timeSteps.html"));
    }

    private void PlotParticleCenter()
    {
        if (_solutionStorage.States.Count == 0)
            return;

        var centers = ProcessPlot.PlotParticleCenters(_solutionStorage.States);
        var start = ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
            _solutionStorage.States[0].Particles
        );
        var end = ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
            _solutionStorage.States[^1].Particles
        );
        var plot = Chart.Combine([centers, start, end]);
        plot.SaveHtml(Path.Combine(_tempDir, "particleCenters.html"));
    }

    private void PlotShrinkage()
    {
        if (_solutionStorage.States.Count == 0)
            return;

        var plot = ProcessPlot.PlotShrinkagesByDistance(_solutionStorage.States);
        plot.SaveHtml(Path.Combine(_tempDir, "shrinkages.html"));
    }

    private void PlotNeckWidths()
    {
        if (_solutionStorage.States.Count == 0)
            return;

        var plot = ProcessPlot.PlotNeckWidths(_solutionStorage.States);
        plot.SaveHtml(Path.Combine(_tempDir, "necks.html"));
    }
}
