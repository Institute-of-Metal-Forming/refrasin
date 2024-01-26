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
    public SolutionState(double time, IEnumerable<Particle> particles, IEnumerable<(Guid from, Guid to)>? contacts = null)
    {
        Time = time;
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
    public double Time { get; }

    /// <inheritdoc cref="ISolutionState.Nodes"/>>
    public IReadOnlyNodeCollection<NodeBase> Nodes { get; }

    /// <inheritdoc cref="ISolutionState.Particles"/>>
    public IReadOnlyParticleCollection<Particle> Particles { get; }

    /// <inheritdoc cref="ISolutionState.Contacts" />
    public IReadOnlyParticleContactCollection<ParticleContact> Contacts { get; }

    IReadOnlyNodeCollection<INode> ISolutionState.Nodes => Nodes;
    IReadOnlyParticleCollection<IParticle> ISolutionState.Particles => Particles;
    IReadOnlyParticleContactCollection<IParticleContact> ISolutionState.Contacts => Contacts;
}