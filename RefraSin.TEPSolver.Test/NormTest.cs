using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using static System.Double;

namespace RefraSin.TEPSolver.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class NormTest(ISystemState<IParticle<IParticleNode>, IParticleNode> state)
{
    public static IEnumerable<TestFixtureData> GetTestFixtureData() =>
        InitialStates.Generate().Select(s => new TestFixtureData(s.state) { TestName = s.label });

    private static readonly ISinteringConditions Conditions = new SinteringConditions(2073, 0);
    private static readonly IParticleMaterial Material = new ParticleMaterial(
        InitialStates.MaterialId,
        "Al2O3",
        SubstanceProperties.FromDensityAndMolarMass(1.8e3, 101.96e-3),
        new InterfaceProperties(1.65e-14, 0.9, 1e-6),
        new Dictionary<Guid, IInterfaceProperties>
        {
            { InitialStates.MaterialId, new InterfaceProperties(1.65e-14, 0.5) },
        }
    );

    [Test]
    public void TestNorm()
    {
        var norm = new DefaultNormalizer().GetNorm(state, Conditions, [Material]);

        Assert.That(norm.Area, Is.EqualTo(Pow(norm.Length, 2)).Within(3).Ulps);
        Assert.That(norm.Volume, Is.EqualTo(Pow(norm.Length, 3)).Within(3).Ulps);
        Assert.That(
            norm.Energy,
            Is.EqualTo(norm.Mass * Pow(norm.Length, 2) / Pow(norm.Time, 2)).Within(3).Ulps
        );
    }
}
