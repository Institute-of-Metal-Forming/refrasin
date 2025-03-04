using Plotly.NET;
using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using static RefraSin.Coordinates.Angle;

namespace RefraSin.Compaction.Tests;

[TestFixture]
[TestFixtureSource(nameof(GenerateFixtureData))]
public class FocalCompactionTests
{
    private readonly AbsolutePoint _focus;
    private readonly SystemState _system;

    public static IEnumerable<TestFixtureData> GenerateFixtureData()
    {
        yield return new TestFixtureData(
            new[]
            {
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (0, 0),
                    0,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (3.0, 0),
                    Straight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
            },
            new AbsolutePoint(0, 0)
        );
        yield return new TestFixtureData(
            new[]
            {
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (0, 0),
                    0,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (-3.0, 0),
                    Straight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
            },
            new AbsolutePoint(0, 0)
        );
        yield return new TestFixtureData(
            new[]
            {
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (-3, -3),
                    HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (3, -3),
                    Straight - HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
            },
            new AbsolutePoint(0, 0)
        );
        yield return new TestFixtureData(
            new[]
            {
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (3, 3),
                    HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (-3, 3),
                    Straight - HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (3, -3),
                    -HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (-3, -3),
                    Straight + HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
            },
            new AbsolutePoint(0, 0)
        );
    }

    public FocalCompactionTests(IEnumerable<Particle<ParticleNode>> particles, AbsolutePoint focus)
    {
        _focus = focus;
        _system = new SystemState(Guid.Empty, 0, particles);
    }

    [Test]
    [TestCase(0.01)]
    [TestCase(0.05)]
    [TestCase(0.10)]
    public void TestCompaction(double stepSize)
    {
        var compactor = new FocalCompactionStep(_focus, stepSize, maxStepCount: 1000);
        var sol = compactor.Solve(_system);

        Chart
            .Combine(
                [
                    ParticlePlot.PlotParticles(
                        _system.Particles.Select(p => new Particle<IParticleNode>(
                            p.Id,
                            sol.Particles[p.Id].Coordinates,
                            p.RotationAngle,
                            p.MaterialId,
                            particle => p.Nodes.Select(n => new ParticleNode(n, particle))
                        ))
                    ),
                    ParticlePlot.PlotParticles(sol.Particles),
                ]
            )
            .Show();
    }
}
