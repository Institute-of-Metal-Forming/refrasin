using Plotly.NET;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.Plotting;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class SurfaceRemeshingTest
{
    private string _tempDir;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.GetTempFileName().Replace(".tmp", "");
        Directory.CreateDirectory(_tempDir);
        TestContext.WriteLine(_tempDir);
    }

    [Test]
    public void TestNodeDeletion()
    {
        var particle = new ShapeFunctionParticleFactory(1, 0.2, 5, 0.1, Guid.Empty)
        {
            NodeCount = 200,
        }.GetParticle();

        var remesher = new FreeSurfaceRemesher(deletionLimit: 0.05);
        var remeshedParticle = remesher.Remesh(particle);

        var plot = ParticlePlot.PlotParticles([particle, remeshedParticle]);
        plot.SaveHtml(Path.Combine(_tempDir, $"{nameof(TestNodeDeletion)}.html"));
    }

    [Test]
    public void TestNodeAddition()
    {
        var particle = new ShapeFunctionParticleFactory(1, 0.2, 5, 0.1, Guid.Empty)
        {
            NodeCount = 20,
        }.GetParticle();

        var remesher = new FreeSurfaceRemesher(deletionLimit: 0.05);
        var remeshedParticle = remesher.Remesh(particle);

        var plot = ParticlePlot.PlotParticles([particle, remeshedParticle]);
        plot.SaveHtml(Path.Combine(_tempDir, $"{nameof(TestNodeAddition)}.html"));
    }
}
