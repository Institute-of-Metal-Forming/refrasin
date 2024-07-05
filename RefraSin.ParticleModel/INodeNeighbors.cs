namespace RefraSin.ParticleModel;

public interface INodeNeighbors : INode
{
    /// <summary>
    /// Upper neighbor of this node.
    /// </summary>
    public INode Upper => Particle.Nodes.UpperNeighborOf(this);

    /// <summary>
    /// Lower neighbor of this node.
    /// </summary>
    public INode Lower => Particle.Nodes.LowerNeighborOf(this);

    /// <summary>
    /// Particle this node belongs to.
    /// </summary>
    public IParticle Particle { get; }
}