using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ParticleModel.System;
using static System.Math;
using static RefraSin.Coordinates.Constants;
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

        foreach (var particleContact in system.ParticleContacts())
        {
            var wasInsertedAtLastNode = false;

            foreach (
                var node in particleContact
                    .FromNodes<IParticle<IParticleNode>, IParticleNode>()
                    .Where(n => n.Type == GrainBoundary)
            )
            {
                if (
                    !wasInsertedAtLastNode
                    && node.SurfaceDistance.ToLower > AdditionLimit * meanDiscretizationWidth
                )
                {
                    AddNodePair(
                        allNodes,
                        particleContact,
                        node.Coordinates.Centroid(node.Lower.Coordinates)
                    );
                }
                wasInsertedAtLastNode = false;

                if (node.SurfaceDistance.ToUpper > AdditionLimit * meanDiscretizationWidth)
                {
                    AddNodePair(
                        allNodes,
                        particleContact,
                        node.Coordinates.Centroid(node.Upper.Coordinates)
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
        IEdge<IParticle<IParticleNode>> particleContact,
        IPolarPoint coordinates
    )
    {
        allNodes.Add(
            new ParticleNode(Guid.NewGuid(), particleContact.From, coordinates, GrainBoundary)
        );
        allNodes.Add(
            new ParticleNode(
                Guid.NewGuid(),
                particleContact.To,
                new PolarPoint(coordinates, particleContact.To),
                GrainBoundary
            )
        );
    }

    public double AdditionLimit { get; } = additionLimit;
}
