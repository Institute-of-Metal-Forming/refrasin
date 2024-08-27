using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Nodes;

public interface IParticleNode : INode, INodeGeometry, INodeNeighbors
{
    public int Index => Particle.Nodes.IndexOf(Id);

    /// <summary>
    /// Particle this node belongs to.
    /// </summary>
    public IParticle<IParticleNode> Particle { get; }

    new IParticleNode Upper => Particle.Nodes.UpperNeighborOf(this);
    INode INodeNeighbors.Upper => Upper;
    new IParticleNode Lower => Particle.Nodes.LowerNeighborOf(this);
    INode INodeNeighbors.Lower => Lower;
}