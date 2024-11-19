using Plotly.NET;
using RefraSin.Coordinates;
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

        _particle1 = new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
        {
            NodeCount = 200,
            RotationAngle = Angle.HalfRight,
            CenterCoordinates = (1, 2),
        }.GetParticle();
        _particle2 = new ShapeFunctionParticleFactory(1.5, 0.2, 5, 0.2, Guid.NewGuid())
        {
            NodeCount = 200,
            RotationAngle = Angle.Straight,
            CenterCoordinates = (2, 3),
        }.GetParticle();
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
        var plot = ParticlePlot.PlotParticles([_particle1, _particle2]);
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
        var edge = new ParticleContactEdge<Particle<ParticleNode>>(_particle1, _particle2);

        var plot1 = ParticlePlot.PlotParticles([_particle1, _particle2]);
        var plot2 = ParticlePlot.PlotContactEdge(edge);
        Chart.Combine([plot1, plot2]).Show();
    }
}
