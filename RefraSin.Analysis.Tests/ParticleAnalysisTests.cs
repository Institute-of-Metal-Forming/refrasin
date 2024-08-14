using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using ScottPlot;

namespace RefraSin.Analysis.Tests;

[TestFixture]
public class ParticleAnalysisTests
{
    private readonly IParticle<IParticleNode> _particle = new ShapeFunctionParticleFactory(
        1,
        0.2,
        5,
        0.2,
        Guid.Empty
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
        var plot = new Plot();
        plot.PlotParticle(_particle);
        plot.PlotPoints(hull);
        plot.SavePng(
            Path.Combine(
                _tempDir,
                $"{nameof(TestConvexHull)}-{TestContext.CurrentContext.Test.ID}"
            ),
            640,
            480
        );
    }
}
