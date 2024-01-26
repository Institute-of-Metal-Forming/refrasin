using RefraSin.ParticleModel;

namespace RefraSin.Storage;

/// <summary>
/// A immutable record of a solution state.
/// </summary>
public record SolutionState : ISolutionState
{
    public SolutionState(double time, IEnumerable<IParticle> particleStates, IEnumerable<IParticleContact> particleContacts)
    {
        Time = time;
        Particles = particleStates.Select(s => s as Particle ?? new Particle(s)).ToParticleCollection();
        Contacts = particleContacts.Select(c => c as ParticleContact ?? new ParticleContact(c)).ToParticleContactCollection();
        Nodes = Particles.SelectMany(p => p.Nodes).ToNodeCollection();
    }

    public SolutionState(ISolutionState state)
    {
        Time = state.Time;
        Particles = state.Particles.Select(s => s as Particle ?? new Particle(s)).ToParticleCollection();
        Contacts = state.Contacts.Select(c => c as ParticleContact ?? new ParticleContact(c)).ToParticleContactCollection();
        Nodes = Particles.SelectMany(p => p.Nodes).ToNodeCollection();
    }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISolutionState.Particles"/>
    public IReadOnlyParticleCollection<Particle> Particles { get; }

    /// <inheritdoc cref="ISolutionState.Nodes"/>
    public IReadOnlyNodeCollection<Node> Nodes { get; }

    /// <inheritdoc cref="ISolutionState.Contacts"/>
    public IReadOnlyParticleContactCollection<ParticleContact> Contacts { get; }

    IReadOnlyParticleCollection<IParticle> ISolutionState.Particles => Particles;
    IReadOnlyNodeCollection<INode> ISolutionState.Nodes => Nodes;
    IReadOnlyParticleContactCollection<IParticleContact> ISolutionState.Contacts => Contacts;
}