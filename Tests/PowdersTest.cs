using System.Linq;
using IMF.Statistics.DistributedProperties;
using NUnit.Framework;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleSources;

namespace Tests;

public class PowdersTest
{
    [Test]
    public void TrigonometricPowderTest()
    {
        var material = new Material(
            "",
            8e6,
            1e6,
            1e6,
            1e-4
        );

        var powder = new TrigonometricPowder(
            material,
            new NormalDistributedProperty(100, 10, minimum: 0),
            new NormalDistributedProperty(0.2, 0.1, 0, 0.5),
            new NormalDistributedProperty(0.2, 0.1, 0, 0.5),
            new CategoricalProperty<uint>(new[]
            {
                new Category<uint>(0.2, 3),
                new Category<uint>(0.5, 5),
                new Category<uint>(0.1, 7),
            })
        );

        var particles = powder.GetParticles(null!, radius => 50).Take(10).ToList();
    }
}