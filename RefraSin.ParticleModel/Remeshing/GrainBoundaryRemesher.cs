using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ParticleModel.System;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Remeshing;

public class GrainBoundaryRemesher(double additionLimit = 2.1) : IParticleSystemRemesher
{
    /// <inheritdoc />
    public IParticleSystem<IParticle<IParticleNode>, IParticleNode> RemeshSystem(
        IParticleSystem<IParticle<IParticleNode>, IParticleNode> system
    )
    {
        var allNodes = new List<IParticleNode>(system.Nodes.Count * 2);
        allNodes.AddRange(system.Nodes);

        var meanDiscretizationWidth = allNodes.Average(n => n.SurfaceDistance.ToUpper);

        foreach (
            var particleContact in system.Particles.EnumerateContactedParticlePairs<
                IParticle<IParticleNode>,
                IParticleNode
            >()
        )
        {
            var wasInsertedAtLastNode = false;

            foreach (
                var nodePair in particleContact
                    .First.EnumerateContactNodePairs<IParticle<IParticleNode>, IParticleNode>(
                        particleContact.Second
                    )
                    .Where(n => n.First.Type is GrainBoundary)
            )
            {
                if (
                    !wasInsertedAtLastNode
                    && nodePair.First.SurfaceDistance.ToLower
                        > AdditionLimit * meanDiscretizationWidth
                )
                {
                    AddNodePair(
                        allNodes,
                        particleContact,
                        nodePair.First.Coordinates.Centroid(nodePair.First.Lower.Coordinates)
                    );
                }
                wasInsertedAtLastNode = false;

                if (
                    nodePair.First.SurfaceDistance.ToUpper
                    > AdditionLimit * meanDiscretizationWidth
                )
                {
                    AddNodePair(
                        allNodes,
                        particleContact,
                        nodePair.First.Coordinates.Centroid(nodePair.First.Upper.Coordinates)
                    );
                    wasInsertedAtLastNode = true;
                }
            }
        }

        return new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(
            allNodes
                .GroupBy(n => n.ParticleId)
                .Select(g =>
                {
                    var template = system.Particles[g.Key];
                    return new Particle<IParticleNode>(
                        g.Key,
                        template.Coordinates,
                        template.RotationAngle,
                        template.MaterialId,
                        p => g.OrderBy(n => n.Coordinates.Phi).Select(n => new ParticleNode(n, p))
                    );
                })
        );
    }

    private static void AddNodePair(
        List<IParticleNode> allNodes,
        UnorderedPair<IParticle<IParticleNode>> particleContact,
        IPolarPoint coordinates
    )
    {
        allNodes.Add(
            new ParticleNode(Guid.NewGuid(), particleContact.First, coordinates, GrainBoundary)
        );
        allNodes.Add(
            new ParticleNode(
                Guid.NewGuid(),
                particleContact.Second,
                new PolarPoint(coordinates, particleContact.Second),
                GrainBoundary
            )
        );
    }

    public double AdditionLimit { get; } = additionLimit;
}
