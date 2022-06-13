using System.Threading;
using IMF.Coordinates.Polar;
using MathNet.Numerics;
using NUnit.Framework;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleSources;
using RefraSin.Core.ParticleTreeSources;
using RefraSin.Core.SinteringProcesses;

namespace Tests;

public class TwoParticleTest
{
    [Test]
    public void RunTwoParticleTest()
    {
        var material = new Material(
            "",
            8e6,
            1e6,
            1e6,
            1e-4
        );

        var materialInterfaces = new MaterialInterfaceCollection()
        {
            new(material, material, 0.7e6, 0.8e6)
        };

        var particleSource = new TrigonometricParticleSource(
            material,
            500,
            0.2,
            0.2,
            5
        );

        var treeSource = new ExplicitTreeSource(
            new ExplicitTreeItem(new PolarPoint(), particleSource, 0,new[]
            {
                new ExplicitTreeItem(new PolarPoint(0, 1500), particleSource, Constants.Pi)
            })
        );

        var process = new SinteringProcess(
            treeSource,
            0,
            1e4,
            materialInterfaces,
            1273
        );

        var solution = process.Solve(CancellationToken.None);
        
        Assert.IsTrue(solution.Succeeded, "solution.Succeeded");
    }
}