using Plotly.NET;
using RefraSin.Compaction.ProcessModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using static MoreLinq.Extensions.TakeEveryExtension;
using static RefraSin.Coordinates.Angle;

namespace RefraSin.Compaction.Tests;

[TestFixture]
[TestFixtureSource(nameof(GenerateFixtureData))]
public class OneByOneCompactionTests(IEnumerable<Particle<ParticleNode>> particles, int dummy)
{
    private readonly SystemState _system = new(Guid.Empty, 0, particles);

    public static IEnumerable<TestFixtureData> GenerateFixtureData()
    {
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
            42
        );
        yield return new TestFixtureData(
            new[]
            {
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (0, 0),
                    HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (4, 0),
                    Straight - HalfRight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
                new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
                    Guid.Empty,
                    (3, -4),
                    Straight,
                    100,
                    1,
                    0.2,
                    5,
                    0.2
                ).GetParticle(),
            },
            42
        );
    }

    [Test]
    // [TestCase(0.01)]
    [TestCase(0.05)]
    // [TestCase(0.10)]
    public void TestCompaction(double stepSize)
    {
        var compactor = new OneByOneCompactionStep(stepSize, maxStepCount: 10000);
        var storage = new InMemorySolutionStorage();
        compactor.UseStorage(storage);
        var sol = compactor.Solve(_system);

        Chart
            .Combine(
                storage
                    .States.TakeEvery(20)
                    .Append(storage.States[^1])
                    .Select(s =>
                        ParticlePlot.PlotParticles<IParticle<IParticleNode>, IParticleNode>(
                            s.Particles
                        )
                    )
            )
            .Show();
    }
}
