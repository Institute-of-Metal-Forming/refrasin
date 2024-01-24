using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using static MoreLinq.Extensions.IndexExtension;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepVectors;
using ScottPlot;
using Serilog;
using static System.Math;
using static NUnit.Framework.Assert;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class TwoParticleTest
{
    [SetUp]
    public void Setup()
    {
        var endTime = 1e1;
        var initialNeck = 2 * PI / 100 / 2 * 120e-6;
        var nodeCountPerParticle = 20;

        var baseParticle1 = new ShapeFunctionParticleFactory(100e-6, 0.1, 5, 0.1, Guid.NewGuid())
                { NodeCount = nodeCountPerParticle }
            .GetParticle();
        var nodes1 = baseParticle1.Nodes.Skip(1).Concat(new[]
        {
            new Node(Guid.NewGuid(), baseParticle1.Id, new PolarPoint(new AbsolutePoint(120e-6, -initialNeck)), NodeType.NeckNode),
            new Node(Guid.NewGuid(), baseParticle1.Id, new PolarPoint(new AbsolutePoint(120e-6, 0)), NodeType.GrainBoundaryNode),
            new Node(Guid.NewGuid(), baseParticle1.Id, new PolarPoint(new AbsolutePoint(120e-6, initialNeck)), NodeType.NeckNode),
        }).ToArray();
        _particle1 = new RefraSin.ParticleModel.Particle(baseParticle1.Id, new(0, 0), 0, baseParticle1.MaterialId, nodes1);

        var baseParticle2 = new ShapeFunctionParticleFactory(100e-6, 0.1, 5, 0.1, _particle1.MaterialId)
                { NodeCount = nodeCountPerParticle }
            .GetParticle();
        var nodes2 = baseParticle2.Nodes.Skip(1).Concat(new[]
        {
            new Node(Guid.NewGuid(), baseParticle2.Id, new PolarPoint(new AbsolutePoint(120e-6, -initialNeck)), NodeType.NeckNode),
            new Node(Guid.NewGuid(), baseParticle2.Id, new PolarPoint(new AbsolutePoint(120e-6, 0)), NodeType.GrainBoundaryNode),
            new Node(Guid.NewGuid(), baseParticle2.Id, new PolarPoint(new AbsolutePoint(120e-6, initialNeck)), NodeType.NeckNode),
        }).ToArray();
        _particle2 = new RefraSin.ParticleModel.Particle(baseParticle2.Id, new(240e-6, 0), PI, baseParticle2.MaterialId, nodes2);

        _solutionStorage = new InMemorySolutionStorage();

        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddFile(Path.Combine(_tempDir, "test.log")); });

        _solver = new Solver
        {
            LoggerFactory = loggerFactory,
            Options = new SolverOptions
            {
                InitialTimeStepWidth = 1,
                MinTimeStepWidth = 0.1,
                TimeStepAdaptationFactor = 1.5,
            },
            SolutionStorage = _solutionStorage,
            StepValidators = new IStepValidator[] { },
        };

        _material = new Material(
            _particle1.MaterialId,
            "Al2O3",
            1.65e-10,
            0,
            1e-4,
            0.9,
            1.8e3,
            101.96e-3
        );

        _materialInterface = new MaterialInterface(
            _material.Id,
            _material.Id,
            0.5,
            1.65e-10,
            0
        );

        _process = new SinteringProcess(
            0,
            endTime,
            new[] { _particle1, _particle2 },
            new[] { _material },
            new[] { _materialInterface },
            2073
        );
    }

    private IParticle _particle1;
    private IParticle _particle2;
    private Solver _solver;
    private IMaterial _material;
    private IMaterialInterface _materialInterface;
    private ISinteringProcess _process;
    private InMemorySolutionStorage _solutionStorage;
    private string _tempDir;

    [Test]
    public void PlotJacobianStructure()
    {
        var session = new SolverSession(_solver, _process);
        var initialState = session.CurrentState;
        var guess = LagrangianGradient.GuessSolution(initialState);

        var matrix = Matrix<double>.Build.DenseOfColumns(YieldJacobianColumns(session, initialState, guess)).PointwiseSign();

        var plt = new Plot();

        plt.Add.Heatmap(matrix.ToArray());
        plt.Layout.Frameless();
        plt.Axes.Margins(0, 0);

        plt.SavePng(Path.Combine(_tempDir, "jacobian.png"), matrix.ColumnCount, matrix.RowCount);
    }

    private IEnumerable<Vector<double>> YieldJacobianColumns(SolverSession session, SolutionState state, StepVector guess)
    {
        var zero = LagrangianGradient.EvaluateAt(session, state, guess);

        for (int i = 0; i < guess.Count; i++)
        {
            var step = guess.Copy();
            step[i] += 1;
            var current = LagrangianGradient.EvaluateAt(session, state, step);

            yield return current - zero;
        }
    }

    [Test]
    public void TestSolution()
    {
        try
        {
            _solver.Solve(_process);
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

            foreach (var particle in state.ParticleStates)
            {
                var coordinates = particle.Nodes
                    .Append(particle.Nodes[0])
                    .Select(n => new ScottPlot.Coordinates(n.Coordinates.Absolute.X, n.Coordinates.Absolute.Y))
                    .ToArray();
                plt.Add.Scatter(coordinates);
            }

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

            foreach (var particle in step.ParticleTimeSteps)
            {
                var coordinates = particle.NodeTimeSteps.Values
                    .Select((n, j) => new ScottPlot.Coordinates(j, n.NormalDisplacement))
                    .ToArray();
                plt.Add.Scatter(coordinates);
            }

            plt.Add.Line(0, 0, step.ParticleTimeSteps[0].NodeTimeSteps.Count, 0);

            plt.Title($"t = {step.StartTime.ToString(CultureInfo.InvariantCulture)} - {step.EndTime.ToString(CultureInfo.InvariantCulture)}");

            plt.SavePng(Path.Combine(dir, $"{i}.png"), 600, 400);
        }
    }

    private void PlotTimeSteps()
    {
        var plt = new Plot();

        var steps = _solutionStorage.Steps.Select(s => new ScottPlot.Coordinates(s.StartTime, s.TimeStepWidth)).ToArray();
        plt.Add.Scatter(steps);

        var meanStepWidth = steps.Select(s => s.Y).Mean();
        plt.Add.Line(0, meanStepWidth, _process.EndTime, meanStepWidth);

        plt.SavePng(Path.Combine(_tempDir, "timeSteps.png"), 600, 400);
    }

    private void PlotParticleCenter()
    {
        var plt = new Plot();

        plt.Add.Scatter(_solutionStorage.States.Select(s =>
            new ScottPlot.Coordinates(s.Time, s.ParticleStates[0].CenterCoordinates.Absolute.X)
        ).ToArray());
        plt.Add.Scatter(_solutionStorage.States.Select(s =>
            new ScottPlot.Coordinates(s.Time, s.ParticleStates[0].CenterCoordinates.Absolute.Y)
        ).ToArray());
        plt.Add.Scatter(_solutionStorage.States.Select(s =>
            new ScottPlot.Coordinates(s.Time, s.ParticleStates[1].CenterCoordinates.Absolute.X)
        ).ToArray());
        plt.Add.Scatter(_solutionStorage.States.Select(s =>
            new ScottPlot.Coordinates(s.Time, s.ParticleStates[1].CenterCoordinates.Absolute.Y)
        ).ToArray());

        plt.SavePng(Path.Combine(_tempDir, "particleCenter.png"), 600, 400);
    }
}