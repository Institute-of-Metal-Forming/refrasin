using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using Plotly.NET;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using ScottPlot;
using static System.Math;
using static MoreLinq.Extensions.IndexExtension;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class TwoParticleTest
{
    [SetUp]
    public void Setup()
    {
        var duration = 1e4;
        var initialNeck = 2 * PI / 100 / 2 * 120e-6 * 5;
        var nodeCountPerParticle = 20;

        var baseParticle1 = new ShapeFunctionParticleFactory(100e-6, 0.1, 5, 0.1, Guid.NewGuid())
        {
            NodeCount = nodeCountPerParticle
        }.GetParticle();

        IEnumerable<IParticleNode> NodeFactory1(IParticle<IParticleNode> particle) =>
            baseParticle1
                .Nodes.Skip(1)
                .Select(n => new ParticleNode(n, particle))
                .Concat(
                    [
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(120e-6, -initialNeck)),
                            NodeType.Neck
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(120e-6, -initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(125e-6, -0.5 * initialNeck)),
                            // new PolarPoint(new AbsolutePoint(125e-6, 0)),
                            // new PolarPoint(new AbsolutePoint(120e-6, 0)),
                            NodeType.GrainBoundary
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(120e-6, initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(120e-6, initialNeck)),
                            NodeType.Neck
                        ),
                    ]
                );

        _particle1 = new Particle<IParticleNode>(
            baseParticle1.Id,
            new AbsolutePoint(0, 0),
            0,
            baseParticle1.MaterialId,
            NodeFactory1
        );

        var baseParticle2 = new ShapeFunctionParticleFactory(
            200e-6,
            0.1,
            5,
            0.1,
            _particle1.MaterialId
        )
        {
            NodeCount = nodeCountPerParticle
        }.GetParticle();

        IEnumerable<IParticleNode> NodeFactory2(IParticle<IParticleNode> particle) =>
            baseParticle2
                .Nodes.Skip(1)
                .Select(n => new ParticleNode(n, particle))
                .Concat(
                    [
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(240e-6, -initialNeck)),
                            NodeType.Neck
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(240e-6, -initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(235e-6, 0.5 * initialNeck)),
                            // new PolarPoint(new AbsolutePoint(235e-6, 0)),
                            // new PolarPoint(new AbsolutePoint(240e-6, 0)),
                            NodeType.GrainBoundary
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(240e-6, initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(240e-6, initialNeck)),
                            NodeType.Neck
                        ),
                    ]
                );

        _particle2 = new Particle<IParticleNode>(
            baseParticle2.Id,
            new AbsolutePoint(360e-6, 0),
            PI,
            baseParticle2.MaterialId,
            NodeFactory2
        );

        _solutionStorage = new InMemorySolutionStorage();

        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddFile(Path.Combine(_tempDir, "test.log")); });

        _solver = new SinteringSolver(_solutionStorage, loggerFactory, SolverRoutines.Default, 30);

        _material = new Material(
            _particle1.MaterialId,
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

        _initialState = new SystemState(Guid.NewGuid(), 0, new[] { _particle1, _particle2 });

        _sinteringProcess = new SinteringStep(
            duration,
            2073,
            _solver,
            new[] { _material },
            new[] { _materialInterface }
        );
        _sinteringProcess.UseStorage(_solutionStorage);
    }

    private IParticle<IParticleNode> _particle1;
    private IParticle<IParticleNode> _particle2;
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
        Chart.Combine([centers, start, end]).SaveHtml(Path.Combine(_tempDir, "particleCenters.html"));
    }
}