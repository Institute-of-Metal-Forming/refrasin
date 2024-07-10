using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Nodes;

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

    public int Index => Particle.Nodes.IndexOf(Id);

    /// <summary>
    /// Particle this node belongs to.
    /// </summary>
    public IParticle Particle { get; }
}