using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver;

public class SolutionState : ISystemState
{
    public SolutionState(Guid id, double time, IEnumerable<Particle> particles, IReadOnlyList<IMaterial> materials,
        IReadOnlyList<IMaterialInterface> materialInterfaces, IEnumerable<(Guid from, Guid to)>? contacts = null)
    {
        Time = time;
        Materials = materials;
        MaterialInterfaces = materialInterfaces;
        Id = id;
        Particles = particles as IReadOnlyParticleCollection<Particle> ?? new ReadOnlyParticleCollection<Particle>(particles);
        Nodes = Particles.SelectMany(p => p.Nodes).ToNodeCollection();

        contacts ??= GetParticleContacts();
        Contacts = contacts.Select(t => new ParticleContact(Particles[t.from], Particles[t.to])).ToParticleContactCollection();
    }

    private IEnumerable<(Guid from, Guid to)> GetParticleContacts()
    {
        var edges = Particles.SelectMany(p => p.Nodes.OfType<NeckNode>())
            .Select(n => new UndirectedEdge<Particle>(n.Particle, n.ContactedNode.Particle));
        var graph = new UndirectedGraph<Particle>(Particles, edges);
        var explorer = BreadthFirstExplorer<Particle>.Explore(graph, Particles[0]);

        return explorer.TraversedEdges.Select(e => (e.From.Id, e.To.Id)).ToArray();
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISystemState.Nodes"/>>
    public IReadOnlyNodeCollection<NodeBase> Nodes { get; }

    /// <inheritdoc cref="ISystemState.Particles"/>>
    public IReadOnlyParticleCollection<Particle> Particles { get; }

    /// <inheritdoc cref="ISystemState.Contacts" />
    public IReadOnlyParticleContactCollection<ParticleContact> Contacts { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterial> Materials { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }

    IReadOnlyNodeCollection<INode> ISystemState.Nodes => Nodes;
    IReadOnlyParticleCollection<IParticle> ISystemState.Particles => Particles;
    IReadOnlyParticleContactCollection<IParticleContact> ISystemState.Contacts => Contacts;
}