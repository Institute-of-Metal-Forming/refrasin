using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Polar;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.Storage;
using static NUnit.Framework.Assert;

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
        _materialRegistry = new MaterialRegistry();
        _solver.MaterialRegistry = _materialRegistry;

        var material1 = new Material(
            _particleSpec.MaterialId,
            "Al2O3",
            1e-9,
            0,
            1e-4,
            1,
            1.8e3,
            101.96e-3
        );
        _materialRegistry.RegisterMaterial(material1);

        var materialInterface1 = new MaterialInterface(
            material1.Id,
            material1.Id,
            0.5,
            1e-9,
            0
        );
        _materialRegistry.RegisterMaterialInterface(materialInterface1);

        _initialState = new SolutionState(
            0,
            new[]
            {
                new Particle(
                    _particleSpec.Id,
                    new PolarPoint(_particleSpec.AbsoluteCenterCoordinates),
                    _particleSpec.AbsoluteCenterCoordinates,
                    _particleSpec.RotationAngle,
                    _particleSpec.MaterialId,
                    _particleSpec.NodeSpecs.Select(ns => new SurfaceNode(
                        ns.Id,
                        _particleSpec.Id,
                        ns.Coordinates,
                        ns.Coordinates.Absolute,
                        new ToUpperToLower(1, 1),
                        new ToUpperToLowerAngle(1, 1),
                        new ToUpperToLowerAngle(1, 1),
                        new ToUpperToLower(1, 1),
                        new NormalTangentialAngle(1, 1),
                        new ToUpperToLower(1, 1),
                        new ToUpperToLower(1, 1),
                        new NormalTangential(1, 1),
                        new NormalTangential(1, 1)
                    )).ToArray()
                )
            }
        );
    }

    private IParticleSpec _particleSpec;
    private Solver _solver;
    private MaterialRegistry _materialRegistry;
    private ISolutionState _initialState;

    [Test]
    public void TestCreateSession()
    {
        var session = _solver.CreateSession(_initialState, 1);
        var particle = session.Particles.Values.First();

        That(particle.Id, Is.EqualTo(_particleSpec.Id));
        That(particle.Material, Is.EqualTo(_materialRegistry.Materials[0]));
        That(particle.Nodes, Has.Count.EqualTo(100));
    }
}