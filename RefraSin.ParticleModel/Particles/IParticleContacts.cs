using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public interface IParticleContacts<out TParticle> : IParticleContacts
    where TParticle : IParticle
{
    new IReadOnlyContactCollection<IParticleContactEdge<TParticle>> Contacts { get; }

    IReadOnlyContactCollection<IParticleContactEdge> IParticleContacts.Contacts => Contacts;
}

public interface IParticleContacts
{
    IReadOnlyContactCollection<IParticleContactEdge> Contacts { get; }
}
