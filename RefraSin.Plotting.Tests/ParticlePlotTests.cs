using Plotly.NET;
using RefraSin.Coordinates;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Plotting.Tests;

[TestFixture]
public class ParticlePlotTests
{
    private readonly string _tempDir;
    private readonly Particle<ParticleNode> _particle1;
    private readonly Particle<ParticleNode> _particle2;

    public ParticlePlotTests()
    {
        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);

        _particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (1, 2),
            Angle.HalfRight,
            200,
            1,
            0.2,
            5,
            0.2
        ).GetParticle();
        _particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            Guid.Empty,
            (2, 3),
            Angle.Straight,
            200,
            1.5,
            0.2,
            5,
            0.2
        ).GetParticle();
    }

    [Test]
    public void TestPlotParticle()
    {
        var plot = ParticlePlot.PlotParticle(_particle1);
        plot.Show();
    }

    [Test]
    public void TestPlotParticles()
    {
        var plot = ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
            [_particle1, _particle2]
        );
        plot.Show();
    }

    [Test]
    public void TestPlotPoint()
    {
        var plot1 = ParticlePlot.PlotParticle(_particle1);
        var plot2 = ParticlePlot.PlotPoint(_particle1.Coordinates, "Center");
        Chart.Combine([plot1, plot2]).Show();
    }

    [Test]
    public void TestPlotPoints()
    {
        var plot = ParticlePlot.PlotPoints(_particle1.Nodes.Select(n => n.Coordinates), "Nodes");
        plot.Show();
    }

    [Test]
    public void TestPlotContactEdge()
    {
        var edge = new ContactPair<Particle<ParticleNode>>(Guid.Empty, _particle1, _particle2);

        var plot1 = ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
            [_particle1, _particle2]
        );
        var plot2 = ParticlePlot.PlotContactEdge(edge);
        Chart.Combine([plot1, plot2]).Show();
    }
}
