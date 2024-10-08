using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.System;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

public class SolutionState : ISystemState<Particle, NodeBase>
{
    public SolutionState(Guid id, double time, ISystemState state, IReadOnlyDictionary<Guid, IMaterial> materials, double temperature, double gasConstant)
    {
        Time = time;
        Id = id;
        Particles = state
            .Particles.Select(ps => new Particle(ps, this, temperature, gasConstant))
            .ToReadOnlyParticleCollection<Particle, NodeBase>();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();

        NodeContacts = state
            .NodeContacts.Select(c => new Edge<ContactNodeBase>(
                (ContactNodeBase)Nodes[c.From.Id],
                (ContactNodeBase)Nodes[c.To.Id],
                true
            ))
            .ToReadOnlyContactCollection();
        ParticleContacts = state
            .ParticleContacts.Select(c => new ParticleContact(
                Particles[c.From.Id],
                Particles[c.To.Id]
            ))
            .ToReadOnlyContactCollection();
        Materials = materials;
    }


    private SolutionState(
        Guid id,
        double time,
        IEnumerable<Particle> particles,
        IEnumerable<ParticleContact> particleContacts,
        IEnumerable<IEdge<ContactNodeBase>> nodeContacts,
        IReadOnlyDictionary<Guid, IMaterial> materials
    )
    {
        Time = time;
        Id = id;
        Particles = particles.ToReadOnlyParticleCollection<Particle, NodeBase>();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();

        NodeContacts = nodeContacts
            .Select(c => new Edge<ContactNodeBase>(
                (ContactNodeBase)Nodes[c.From.Id],
                (ContactNodeBase)Nodes[c.To.Id],
                true
            ))
            .ToReadOnlyContactCollection();
        ParticleContacts = particleContacts
            .Select(c => new ParticleContact(Particles[c.From.Id], Particles[c.To.Id]))
            .ToReadOnlyContactCollection();
        Materials = materials;
    }
    
    public IReadOnlyDictionary<Guid,IMaterial> Materials { get; set; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISystemState.Nodes"/>>
    public IReadOnlyNodeCollection<NodeBase> Nodes { get; }

    /// <inheritdoc cref="ISystemState.Particles"/>>
    public IReadOnlyParticleCollection<Particle, NodeBase> Particles { get; }

    public IReadOnlyContactCollection<ParticleContact> ParticleContacts { get; }

    public IReadOnlyContactCollection<IEdge<ContactNodeBase>> NodeContacts { get; }

    IReadOnlyNodeCollection<NodeBase> IParticleSystem<Particle, NodeBase>.Nodes => Nodes;
    IReadOnlyParticleCollection<Particle, NodeBase> IParticleSystem<Particle, NodeBase>.Particles =>
        Particles;
    IReadOnlyContactCollection<IParticleContactEdge<Particle>> IParticleSystem<
        Particle,
        NodeBase
    >.ParticleContacts => ParticleContacts;

    IReadOnlyContactCollection<IEdge<NodeBase>> IParticleSystem<Particle, NodeBase>.NodeContacts =>
        NodeContacts;

    public SolutionState ApplyTimeStep(StepVector stepVector, double timeStepWidth)
    {
        var newParticles = new Dictionary<Guid, Particle>()
        {
            [Particles.Root.Id] = Particles.Root.ApplyTimeStep(null, stepVector, timeStepWidth)
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
            ParticleContacts,
            NodeContacts,
            Materials
        );

        return newState;
    }

    public void Sanitize()
    {
        var newNodeCoordinates = NodeContacts
            .Select(e =>
            {
                var node = Nodes[e.From.Id];
                var contactedNode = Nodes[e.To.Id];
                var halfway = node.Coordinates.Centroid(contactedNode.Coordinates);
                return (node, halfway);
            })
            .ToArray();

        foreach (var (node, halfway) in newNodeCoordinates)
        {
            node.Coordinates = halfway;
        }
    }
}
