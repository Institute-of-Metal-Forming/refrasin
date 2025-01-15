using RefraSin.Graphs;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;

namespace RefraSin.ParticleModel.System;

public static class ParticleSystemExtensions
{
    public static IEnumerable<IEdge<TParticle>> ParticleContacts<TParticle, TNode>(
        this IParticleSystem<TParticle, TNode> self
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var graph = new DirectedGraph<TParticle, Edge<TParticle>>(
            self.Particles,
            self.Particles.SelectMany(p =>
                p.ContactedParticles<TParticle, TNode>(self.Particles.Except([p]))
                    .Select(p2 => new Edge<TParticle>(p, p2))
            )
        );
        var search = BreadthFirstExplorer<TParticle, Edge<TParticle>>.Explore(
            graph,
            self.Particles[0]
        );
        return search.TraversedEdges;
    }
}
