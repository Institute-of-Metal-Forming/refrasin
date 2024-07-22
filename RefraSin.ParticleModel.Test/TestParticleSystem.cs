using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class TestParticleSystem
{
    private ParticleSystem<Particle<ParticleNode>, ParticleNode> _system;

    [SetUp]
    public void Setup()
    {
        var nodeCountPerParticle = 20;
        var initialNeck = 5e-6;

        var baseParticle1 = new ShapeFunctionParticleFactory(100e-6, 0.1, 5, 0.1, Guid.NewGuid())
        {
            NodeCount = nodeCountPerParticle
        }.GetParticle();

        IEnumerable<ParticleNode> NodeFactory1(IParticle<ParticleNode> particle) =>
            baseParticle1
                .Nodes.Skip(1)
                .Select(n => new ParticleNode(n, particle))
                .Concat(
                    [
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(120e-6, -initialNeck)),
                            NodeType.Neck
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(120e-6, -initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(125e-6, -0.5 * initialNeck)),
                            // new PolarPoint(new AbsolutePoint(125e-6, 0)),
                            // new PolarPoint(new AbsolutePoint(120e-6, 0)),
                            NodeType.GrainBoundary
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(120e-6, initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(120e-6, initialNeck)),
                            NodeType.Neck
                        ),
                    ]
                );

        var particle1 = new Particle<ParticleNode>(
            baseParticle1.Id,
            new AbsolutePoint(0, 0),
            0,
            baseParticle1.MaterialId,
            NodeFactory1
        );

        var baseParticle2 = new ShapeFunctionParticleFactory(
            200e-6,
            0.1,
            5,
            0.1,
            particle1.MaterialId
        )
        {
            NodeCount = nodeCountPerParticle
        }.GetParticle();

        IEnumerable<ParticleNode> NodeFactory2(IParticle<ParticleNode> particle) =>
            baseParticle2
                .Nodes.Skip(1)
                .Select(n => new ParticleNode(n, particle))
                .Concat(
                    [
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(240e-6, -initialNeck)),
                            NodeType.Neck
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(240e-6, -initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(235e-6, 0.5 * initialNeck)),
                            // new PolarPoint(new AbsolutePoint(235e-6, 0)),
                            // new PolarPoint(new AbsolutePoint(240e-6, 0)),
                            NodeType.GrainBoundary
                        ),
                        // new ParticleNode(
                        //     Guid.NewGuid(),
                        //     particle,
                        //     new PolarPoint(new AbsolutePoint(240e-6, initialNeck / 2)),
                        //     NodeType.GrainBoundary
                        // ),
                        new ParticleNode(
                            Guid.NewGuid(),
                            particle,
                            new PolarPoint(new AbsolutePoint(240e-6, initialNeck)),
                            NodeType.Neck
                        ),
                    ]
                );

        var particle2 = new Particle<ParticleNode>(
            baseParticle2.Id,
            new AbsolutePoint(360e-6, 0),
            Pi,
            baseParticle2.MaterialId,
            NodeFactory2
        );

        _system = new ParticleSystem<Particle<ParticleNode>, ParticleNode>(new[] { particle1, particle2 });
    }
}