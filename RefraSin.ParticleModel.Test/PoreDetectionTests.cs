using Plotly.NET;
using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ParticleModel.System;
using RefraSin.Plotting;
using RefraSin.ProcessModel;

namespace RefraSin.ParticleModel.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class PoreDetectionTests(
    IParticleSystem<IParticle<IParticleNode>, IParticleNode> system,
    int poreCount
)
{
    public static IEnumerable<TestFixtureData> GetTestFixtureData()
    {
        yield return new TestFixtureData(FourParticleRing(), 1);
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> FourParticleRing()
    {
        var nodeCountPerParticle = 40;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (-130e-6, -130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (-130e-6, 130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var particle3 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (130e-6, -130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var particle4 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (130e-6, 130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var initialState = new SystemState(
            Guid.Empty,
            0,
            [particle1, particle2, particle3, particle4]
        );
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 2e-6, 1.5e-6).Solve(
            initialState
        );

        return compactedState;
    }

    [Test]
    public void TestPoreDetection()
    {
        var pores = system
            .Particles.DetectPores<IParticle<IParticleNode>, IParticleNode>()
            .ToArray();

        var poresWithout = pores
            .WithoutOuterSurface<IPore<IParticleNode>, IParticleNode>()
            .ToArray();

        var particlePlot = ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
            system.Particles
        );
        var porePlots = poresWithout.Select(p =>
            ParticlePlot.PlotPatch(
                p.Nodes.Select(n => n.Coordinates),
                $"pore {p.Id
            .ToString()[..8]}"
            )
        );

        var plot = Chart.Combine(porePlots.Append(particlePlot));

        var path = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            $"pores-{Guid.NewGuid()}.html"
        );
        plot.SaveHtml(path);
        TestContext.WriteLine($"file://{path}");

        Assert.That(pores, Has.Length.EqualTo(poreCount + 1));
        Assert.That(poresWithout, Has.Length.EqualTo(poreCount));
        Assert.That(poresWithout.Select(p => p.Volume), Is.All.Positive);
    }
}
