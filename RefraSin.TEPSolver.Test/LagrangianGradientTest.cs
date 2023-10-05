using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Absolute;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.ParticleSpecFactories;
using RefraSin.ProcessModel;

namespace RefraSin.TEPSolver.Test;

[TestFixture]
public class LagrangianGradientTest
{
    private SolverSession _solverSession;
    private LagrangianGradient _gradient;
    private IParticleSpec _particleSpec1;
    private IParticleSpec _particleSpec2;

    [SetUp]
    public void Setup()
    {
        var material = new Material(
            Guid.NewGuid(),
            "Al2O3",
            1e-9,
            0,
            1e-4,
            1,
            1.8e3,
            101.96e-3
        );

        var materialInterface = new MaterialInterface(
            material.Id,
            material.Id,
            0.5,
            1e-9,
            0
        );

        _particleSpec1 = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, material.Id).GetParticleSpec();
        _particleSpec2 = new ShapeFunctionParticleSpecFactory(100, 0.1, 5, 0.1, material.Id)
                { CenterCoordinates = new AbsolutePoint(200, 0) }
            .GetParticleSpec();

        var solver = new Solver
        {
            LoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); }),
            Options = new SolverOptions
            {
                InitialTimeStepWidth = 0.01
            }
        };

        var process = new SinteringProcess(
            0,
            1,
            new[] { _particleSpec1, _particleSpec2 },
            new[] { material },
            new[] { materialInterface },
            1273
        );

        _solverSession = solver.CreateSession(process);
        _gradient = _solverSession.LagrangianGradient;
    }

    [Test]
    public void TestGlobalIndices()
    {
        Assert.That(_gradient.GetIndex(LagrangianGradient.GlobalUnknown.Lambda1), Is.EqualTo(0));
    }

    [Test]
    public void TestNodeIndices()
    {
        // first particle

        // first node
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[0].Id, LagrangianGradient.NodeUnknown.NormalDisplacement), Is.EqualTo(1));
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[0].Id, LagrangianGradient.NodeUnknown.FluxToUpper), Is.EqualTo(2));
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[0].Id, LagrangianGradient.NodeUnknown.Lambda2), Is.EqualTo(3));

        // second node
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[1].Id, LagrangianGradient.NodeUnknown.NormalDisplacement), Is.EqualTo(4));
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[1].Id, LagrangianGradient.NodeUnknown.FluxToUpper), Is.EqualTo(5));
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[1].Id, LagrangianGradient.NodeUnknown.Lambda2), Is.EqualTo(6));

        // last node
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[99].Id, LagrangianGradient.NodeUnknown.NormalDisplacement), Is.EqualTo(298));
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[99].Id, LagrangianGradient.NodeUnknown.FluxToUpper), Is.EqualTo(299));
        Assert.That(_gradient.GetIndex(_particleSpec1.NodeSpecs[99].Id, LagrangianGradient.NodeUnknown.Lambda2), Is.EqualTo(300));

        // second particle

        // first node
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[0].Id, LagrangianGradient.NodeUnknown.NormalDisplacement), Is.EqualTo(301));
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[0].Id, LagrangianGradient.NodeUnknown.FluxToUpper), Is.EqualTo(302));
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[0].Id, LagrangianGradient.NodeUnknown.Lambda2), Is.EqualTo(303));

        // second node
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[1].Id, LagrangianGradient.NodeUnknown.NormalDisplacement), Is.EqualTo(304));
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[1].Id, LagrangianGradient.NodeUnknown.FluxToUpper), Is.EqualTo(305));
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[1].Id, LagrangianGradient.NodeUnknown.Lambda2), Is.EqualTo(306));

        // last node
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[99].Id, LagrangianGradient.NodeUnknown.NormalDisplacement), Is.EqualTo(598));
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[99].Id, LagrangianGradient.NodeUnknown.FluxToUpper), Is.EqualTo(599));
        Assert.That(_gradient.GetIndex(_particleSpec2.NodeSpecs[99].Id, LagrangianGradient.NodeUnknown.Lambda2), Is.EqualTo(600));
    }
}