using System.Globalization;
using Microsoft.Extensions.Logging;
using static MoreLinq.Extensions.IndexExtension;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using ScottPlot;
using static NUnit.Framework.Assert;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class OneParticleTest
{
    [SetUp]
    public void Setup()
    {
        var endTime = 1e4;

        _particleSpec = new ShapeFunctionParticleSpecFactory(100e-6, 0.1, 5, 0.1, Guid.NewGuid()).GetParticleSpec();
        _solutionStorage = new InMemorySolutionStorage();

        _solver = new Solver
        {
            LoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); }),
            Options = new SolverOptions
            {
                InitialTimeStepWidth = 1,
                MinTimeStepWidth = 0.1,
            },
            SolutionStorage = _solutionStorage
        };

        _material = new Material(
            _particleSpec.MaterialId,
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
            new[] { _particleSpec },
            new[] { _material },
            new[] { _materialInterface },
            2073
        );
    }

    private IParticleSpec _particleSpec;
    private Solver _solver;
    private IMaterial _material;
    private IMaterialInterface _materialInterface;
    private ISinteringProcess _process;
    private InMemorySolutionStorage _solutionStorage;

    [Test]
    public void TestCreateSession()
    {
        var session = _solver.CreateSession(_process);
        var particle = session.Particles.Values.First();

        That(particle.Id, Is.EqualTo(_particleSpec.Id));
        That(particle.Material, Is.EqualTo(_material));
        That(particle.Nodes, Has.Count.EqualTo(100));
        That(particle, Is.TypeOf<Particle>());
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
            var dir = Path.GetTempFileName().Replace(".tmp", "");
            Directory.CreateDirectory(dir);
            TestContext.WriteLine(dir);

            foreach (var (i, state) in _solutionStorage.States.Index())
            {
                var plt = new Plot();

                var coordinates = state.ParticleStates[0].Nodes
                    .Append(state.ParticleStates[0].Nodes[0])
                    .Select(n => new ScottPlot.Coordinates(n.AbsoluteCoordinates.X, n.AbsoluteCoordinates.Y))
                    .ToArray();
                plt.Add.Scatter(coordinates);

                plt.Title($"t = {state.Time.ToString(CultureInfo.InvariantCulture)}");

                plt.SavePng(Path.Combine(dir, $"{i}.png"), 3000, 3000);
            }
        }
    }
}