using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver;

public class SolutionState : ISystemState
{
    public SolutionState(Guid id, double time, IEnumerable<Particle> particles, IEnumerable<(Guid id, Guid from, Guid to)>? particleContacts = null,
        IEnumerable<(Guid from, Guid to)>? nodeContacts = null)
    {
        Time = time;
        Id = id;
        Particles = particles as IReadOnlyParticleCollection<Particle> ?? new ReadOnlyParticleCollection<Particle>(particles);
        Nodes = Particles.SelectMany(p => p.Nodes).ToNodeCollection();

        particleContacts ??= GetParticleContacts();
        ParticleContacts = particleContacts.Select(t => new ParticleContact(t.id, Particles[t.from], Particles[t.to])).ToParticleContactCollection();

        nodeContacts ??= GetNodeContacts();
        NodeContacts = nodeContacts.ToDictionary(t => t.from, t => t.to);
    }

    private IEnumerable<(Guid id, Guid from, Guid to)> GetParticleContacts()
    {
        var edges = Particles.SelectMany(p => p.Nodes.OfType<NeckNode>())
            .Select(n => new UndirectedEdge<Particle>(n.Particle, n.ContactedNode.Particle));
        var graph = new UndirectedGraph<Particle>(Particles, edges);
        var explorer = BreadthFirstExplorer<Particle>.Explore(graph, Particles[0]);

        return explorer.TraversedEdges.Select(e => (e.Id, e.From.Id, e.To.Id)).ToArray();
    }

    private IEnumerable<(Guid from, Guid to)> GetNodeContacts()
    {
        foreach (var contactNode in Nodes.OfType<ContactNodeBase>())
        {
            var contactedNode = Nodes.FirstOrDefault(n =>
                n.Id != contactNode.Id && n.Coordinates.Absolute.IsClose(contactNode.Coordinates.Absolute, 1e-4)
            );

            if (contactedNode is null)
                throw new InvalidOperationException(
                    $"No corresponding node with same location as {contactNode} could be found."
                );

            yield return (contactNode.Id, contactedNode.Id);
        }
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISystemState.Nodes"/>>
    public IReadOnlyNodeCollection<NodeBase> Nodes { get; }

    /// <inheritdoc cref="ISystemState.Particles"/>>
    public IReadOnlyParticleCollection<Particle> Particles { get; }

    public IReadOnlyParticleContactCollection<ParticleContact> ParticleContacts { get; }
    public IReadOnlyDictionary<Guid, Guid> NodeContacts { get; }

    IReadOnlyNodeCollection<INode> ISystemState.Nodes => Nodes;
    IReadOnlyParticleCollection<IParticle> ISystemState.Particles => Particles;

    public SolutionState ApplyTimeStep(StepVector stepVector, double timeStepWidth)
    {
        var newParticles = new Dictionary<Guid, Particle>()
        {
            [Particles.Root.Id] =
                Particles.Root.ApplyTimeStep(
                    null,
                    stepVector,
                    timeStepWidth
                )
        };

        foreach (var contact in ParticleContacts)
        {
            newParticles[contact.To.Id] = contact.To.ApplyTimeStep(
                newParticles[contact.From.Id],
                stepVector,
                timeStepWidth
            );
        }

        var newState = new SolutionState(
            Guid.NewGuid(),
            Time + timeStepWidth,
            newParticles.Values,
            ParticleContacts.Select(c => (c.Id, c.From.Id, c.To.Id)),
            NodeContacts.Select(c => (c.Key, c.Value)).ToArray()
        );

        return newState;
    }

    public void Sanitize()
    {
        var newNodeCoordinates = NodeContacts.Select(kv =>
        {
            var node = Nodes[kv.Key];
            var contactedNode = Nodes[kv.Value];
            var halfway = node.Coordinates.PointHalfWayTo(contactedNode.Coordinates);
            return (node, halfway);
        });
        
        foreach (var (node, halfway) in newNodeCoordinates)
        {
            node.Coordinates.Phi = halfway.Phi;
            node.Coordinates.R = halfway.R;
        }
    }
}