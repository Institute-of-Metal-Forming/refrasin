using RefraSin.Graphs;
using RefraSin.ParticleModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver;

public class SolutionState : ISolutionState
{
    public SolutionState(double time, IEnumerable<Particle> particles, IEnumerable<(Guid from, Guid to)> contacts)
    {
        Time = time;
        Particles = particles as IReadOnlyParticleCollection<Particle> ?? new ReadOnlyParticleCollection<Particle>(particles);
        AllNodes = Particles.SelectMany(p => p.Nodes).ToDictionaryById();
        Contacts = contacts.ToDictionary(c => (c.from, c.to), c => new ParticleContact(Particles[c.from], Particles[c.to]));
    }

    /// <inheritdoc />
    public double Time { get; }

    public IReadOnlyParticleCollection<Particle> Particles { get; }

    public IReadOnlyDictionary<Guid, NodeBase> AllNodes { get; }

    public IReadOnlyDictionary<(Guid from, Guid to), ParticleContact> Contacts { get; }

    /// <inheritdoc />
    IReadOnlyList<IParticle> ISolutionState.ParticleStates => Particles;

    /// <inheritdoc />
    public IReadOnlyList<IParticleContact> ParticleContacts => Contacts.Values.ToArray();
}