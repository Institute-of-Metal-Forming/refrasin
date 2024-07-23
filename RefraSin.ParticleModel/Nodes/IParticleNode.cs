using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Nodes;

public interface IParticleNode : INode, INodeGeometry, INodeNeighbors
{
    public int Index => Particle.Nodes.IndexOf(Id);

    /// <summary>
    /// Particle this node belongs to.
    /// </summary>
    public IParticle<IParticleNode> Particle { get; }
    
    INode INodeNeighbors.Upper => Particle.Nodes.UpperNeighborOf(this);
    INode INodeNeighbors.Lower => Particle.Nodes.LowerNeighborOf(this);
}