using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.Vertex;

namespace RefraSin.ProcessModel.Extensions;

public static class PoreExtensions
{
    public static ISystemStateWithPores<TParticle, TNode, IPoreState<TNode>> DetectPores<
        TParticle,
        TNode
    >(this ISystemState<TParticle, TNode> system, double porePorosity, double poreElasticStrain)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode =>
        new SystemStateWithPores<TParticle, TNode, IPoreState<TNode>>(
            system.Id,
            system.Time,
            system.Particles,
            system
                .Particles.DetectPores<TParticle, TNode>()
                .Select(
                    IPoreState<TNode> (p) =>
                        new PoreState<TNode>(p.Id, p.Nodes, porePorosity, poreElasticStrain)
                )
                .ToReadOnlyVertexCollection()
        );

    public static ISystemStateWithPores<TParticle, TNode, TPore> WithoutOuterSurface<
        TParticle,
        TNode,
        TPore
    >(this ISystemStateWithPores<TParticle, TNode, TPore> system)
        where TNode : IParticleNode
        where TPore : IPoreState<TNode>
        where TParticle : IParticle<TNode> =>
        new SystemStateWithPores<TParticle, TNode, TPore>(
            system.Id,
            system.Time,
            system.Particles,
            system.Pores.WithoutOuterSurface<TPore, TNode>().ToReadOnlyVertexCollection()
        );
}
