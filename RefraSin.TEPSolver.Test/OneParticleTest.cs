using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Polar;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using static NUnit.Framework.Assert;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class OneParticleTest
{
    [SetUp]
    public void Setup()
    {
        _particleSpec = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, Guid.NewGuid()).GetParticleSpec();
        _solver = new Solver();
        _solver.LoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        _solver.SolutionStorage = new InMemorySolutionStorage();

        _material = new Material(
            _particleSpec.MaterialId,
            "Al2O3",
            1e-9,
            0,
            1e-4,
            1,
            1.8e3,
            101.96e-3
        );

        _materialInterface = new MaterialInterface(
            _material.Id,
            _material.Id,
            0.5,
            1e-9,
            0
        );

        _process = new SinteringProcess(
            0,
            1,
            new[] { _particleSpec },
            new[] { _material },
            new[] { _materialInterface }
        );
    }

    private IParticleSpec _particleSpec;
    private Solver _solver;
    private IMaterial _material;
    private IMaterialInterface _materialInterface;
    private ISinteringProcess _process;

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
}