namespace RefraSin.ParticleModel;

public interface INodeNeighbors
{
    /// <summary>
    /// Upper neighbor of this node.
    /// </summary>
    public INode Upper { get; }

    /// <summary>
    /// Lower neighbor of this node.
    /// </summary>
    public INode Lower { get; }

    /// <summary>
    /// Particle this node belongs to.
    /// </summary>
    public IParticle Particle { get; }
}