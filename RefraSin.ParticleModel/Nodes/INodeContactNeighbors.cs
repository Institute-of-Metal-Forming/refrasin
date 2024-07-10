using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Nodes;

public interface INodeContactNeighbors : INodeContact, INodeNeighbors
{
    IParticle ContactedParticle { get; }
    
    INodeContactNeighbors ContactedNode { get; }
}