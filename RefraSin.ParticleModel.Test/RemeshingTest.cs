using System.Globalization;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Remeshing;
using ScottPlot;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class RemeshingTest
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
        var particle = new ShapeFunctionParticleFactory(1, 0.2, 5, 0.1, Guid.Empty) { NodeCount = 200 }.GetParticle();

        var remesher = new FreeSurfaceRemesher(deletionLimit: 0.05);
        var remeshedParticle = remesher.Remesh(particle);

        var plt = new Plot();
        plt.Axes.SquareUnits();

        plt.Add.Scatter(particle
            .Nodes.Append(particle.Nodes[0])
            .Select(n => new ScottPlot.Coordinates(
                n.Coordinates.Absolute.X,
                n.Coordinates.Absolute.Y
            ))
            .ToArray());
        plt.Add.Scatter(remeshedParticle
            .Nodes.Append(particle.Nodes[0])
            .Select(n => new ScottPlot.Coordinates(
                n.Coordinates.Absolute.X,
                n.Coordinates.Absolute.Y
            ))
            .ToArray());

        plt.SavePng(Path.Combine(_tempDir, $"{nameof(TestNodeDeletion)}.png"), 1600, 900);
    }
    [Test]
    public void TestNodeAddition()
    {
        var particle = new ShapeFunctionParticleFactory(1, 0.2, 5, 0.1, Guid.Empty) { NodeCount = 20 }.GetParticle();

        var remesher = new FreeSurfaceRemesher(deletionLimit: 0.05);
        var remeshedParticle = remesher.Remesh(particle);

        var plt = new Plot();
        plt.Axes.SquareUnits();

        plt.Add.Scatter(particle
            .Nodes.Append(particle.Nodes[0])
            .Select(n => new ScottPlot.Coordinates(
                n.Coordinates.Absolute.X,
                n.Coordinates.Absolute.Y
            ))
            .ToArray()).LineWidth = 2 ;
        
        plt.Add.Scatter(remeshedParticle
            .Nodes.Append(particle.Nodes[0])
            .Select(n => new ScottPlot.Coordinates(
                n.Coordinates.Absolute.X,
                n.Coordinates.Absolute.Y
            ))
            .ToArray());

        plt.SavePng(Path.Combine(_tempDir, $"{nameof(TestNodeAddition)}.png"), 1600, 900);
    }
}