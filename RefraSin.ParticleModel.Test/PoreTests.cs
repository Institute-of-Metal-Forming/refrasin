using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;

namespace RefraSin.ParticleModel.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class PoreTests(Pore<Node> pore)
{
    public static IEnumerable<TestFixtureData> GetTestFixtureData()
    {
        yield return new TestFixtureData(
            new Pore<Node>(
                Guid.NewGuid(),
                [
                    new Node(Guid.NewGuid(), Guid.Empty, new PolarPoint(0, 1), NodeType.Surface),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Right, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight + Angle.Right, 1),
                        NodeType.Surface
                    ),
                ]
            )
        );

        yield return new TestFixtureData(
            new Pore<Node>(
                Guid.NewGuid(),
                [
                    new Node(Guid.NewGuid(), Guid.Empty, new PolarPoint(0, 1), NodeType.Surface),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Right, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight, 0),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight + Angle.Right, 1),
                        NodeType.Surface
                    ),
                ]
            )
        );

        yield return new TestFixtureData(
            new Pore<Node>(
                Guid.NewGuid(),
                [
                    new Node(Guid.NewGuid(), Guid.Empty, new PolarPoint(0, 2), NodeType.Surface),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Right, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight, 2),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight + Angle.Quarter, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Full - Angle.Quarter, 1),
                        NodeType.Surface
                    ),
                ]
            )
        );

        yield return new TestFixtureData(
            new Pore<Node>(
                Guid.NewGuid(),
                [
                    new Node(Guid.NewGuid(), Guid.Empty, new PolarPoint(0, 1), NodeType.Surface),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Right, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Right, 1),
                        NodeType.Surface
                    ), // intentionally repeated
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight, 1),
                        NodeType.Surface
                    ),
                    new Node(
                        Guid.NewGuid(),
                        Guid.Empty,
                        new PolarPoint(Angle.Straight + Angle.Right, 1),
                        NodeType.Surface
                    ),
                ]
            )
        );
    }

    [Test]
    public void TestVolume()
    {
        var volume = new Particle<ParticleNode>(
            Guid.Empty,
            new AbsolutePoint(),
            0,
            Guid.Empty,
            particle => pore.Nodes.Select(n => new ParticleNode(n, particle))
        )
            .Nodes.Select(n => n.Volume().ToUpper)
            .Sum();

        Assert.That(pore.Volume<Pore<Node>, Node>(), Is.EqualTo(volume).Within(1e-8));
    }

    [Test]
    public void TestVolumeDifferential()
    {
        var differentials = Enumerable.Repeat(1.0, pore.Nodes.Count).ToArray();

        Assert.That(
            pore.VolumeDifferential<Pore<Node>, Node>(differentials, differentials),
            Is.EqualTo(0).Within(1e-8)
        );
    }
}
