using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;
using Plotly.NET;
using RefraSin.Plotting;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;
using static MoreLinq.Extensions.IndexExtension;

namespace RefraSin.TEPSolver.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class SimulationTest
{
    public SimulationTest(SolutionState initialState)
    {
        _initialState = initialState;

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFile(Path.Combine(_tempDir, "test.log"));
        });

        var solver = new SinteringSolver(loggerFactory, SolverRoutines.Default, 10);

        _sinteringProcess = new SinteringStep(
            Conditions,
            solver,
            initialState.Materials.Values.ToArray()
        );
        _sinteringProcess.UseStorage(_solutionStorage);
        _sinteringProcess.UseStorage(
            new ParquetStorage.ParquetStorage(Path.Combine(_tempDir, "results.parquet"))
        );
    }

    public static IEnumerable<TestFixtureData> GetTestFixtureData() =>
        InitialStates
            .Generate(Conditions)
            .Select(s => new TestFixtureData(s.state) { TestName = s.label });

    private static readonly ISinteringConditions Conditions = new SinteringConditions(2073, 3.6e4);
    private readonly SolutionState _initialState;
    private readonly SinteringStep _sinteringProcess;
    private readonly InMemorySolutionStorage _solutionStorage = new();
    private readonly string _tempDir = TempPath.CreateTempDir();

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
