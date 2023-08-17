using RefraSin.ParticleModel.ParticleSpecFactories;

namespace RefraSin.ParticleModel.Test;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestShapeFunctionSpecFactory()
    {
        var factory = new ShapeFunctionParticleSpecFactory(
            100,
            0.1,
            5,
            0.1,
            Guid.Empty
        );

        var spec = factory.GetParticleSpec();
    }
}