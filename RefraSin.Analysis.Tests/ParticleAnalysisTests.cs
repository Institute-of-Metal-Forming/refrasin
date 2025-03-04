using Plotly.NET;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;

namespace RefraSin.Analysis.Tests;

[TestFixture]
public class ParticleAnalysisTests
{
    private readonly IParticle<IParticleNode> _particle =
        new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            new AbsolutePoint(),
            0,
            100,
            1,
            0.2,
            5,
            0.2
        ).GetParticle();

    private readonly string _tempDir;

    public ParticleAnalysisTests()
    {
        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);
    }

    [Test]
    public void TestEquivalentRadius()
    {
        var equivalentRadius = ParticleAnalysis.EquivalentRadius(_particle);
        Assert.That(equivalentRadius, Is.EqualTo(1.0189).Within(1e-4));
    }

    [Test]
    public void TestCircularity()
    {
        var circularity = ParticleAnalysis.Circularity(_particle);
        Assert.That(circularity, Is.EqualTo(0.8202).Within(1e-4));
    }

    [Test]
    public void TestConvexHull()
    {
        var hull = ParticleAnalysis.ConvexHull(_particle);
        Chart
            .Combine(
                [
                    ParticlePlot.PlotParticle(_particle),
                    ParticlePlot.PlotLineRing(hull, "convex hull"),
                ]
            )
            .Show();
    }
}
