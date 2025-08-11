using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Collections;

public static class CollectionExtensions
{
    /// <summary>
    /// Creates a new <see cref="ReadOnlyParticleSurface{TContact}"/> from the given enumerable.
    /// </summary>
    public static ReadOnlyParticleSurface<TNode> ToReadOnlyParticleSurface<TNode>(
        this IEnumerable<TNode> source
    )
        where TNode : INode => new(source);

    public static ParticleSurfaceEnumerable<IParticleNode> From<TNode>(this TNode self)
        where TNode : IParticleNode =>
        new(self.Particle.Nodes, self, null, SurfaceIterationDirection.Upward);

    public static ParticleSurfaceEnumerable<TNode> From<TNode>(
        this IReadOnlyParticleSurface<TNode> self,
        TNode startNode
    )
        where TNode : INode => new(self, startNode, null, SurfaceIterationDirection.Upward);

    public static ParticleSurfaceEnumerable<TNode> To<TNode>(
        this ParticleSurfaceEnumerable<TNode> self,
        TNode endNode
    )
        where TNode : INode =>
        new(self.Surface, self.StartNode, n => n.Id == endNode.Id, self.Direction);

    public static ParticleSurfaceEnumerable<TNode> While<TNode>(
        this ParticleSurfaceEnumerable<TNode> self,
        Func<TNode, bool> breakCondition
    )
        where TNode : INode => new(self.Surface, self.StartNode, breakCondition, self.Direction);

    public static ParticleSurfaceEnumerable<TNode> Upward<TNode>(
        this ParticleSurfaceEnumerable<TNode> self
    )
        where TNode : INode =>
        new(self.Surface, self.StartNode, self.BreakCondition, SurfaceIterationDirection.Upward);

    public static ParticleSurfaceEnumerable<TNode> Downward<TNode>(
        this ParticleSurfaceEnumerable<TNode> self
    )
        where TNode : INode =>
        new(self.Surface, self.StartNode, self.BreakCondition, SurfaceIterationDirection.Downward);
}
