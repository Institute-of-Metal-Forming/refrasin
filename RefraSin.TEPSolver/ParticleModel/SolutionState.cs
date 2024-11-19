using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.System;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

public class SolutionState : ISystemState<Particle, NodeBase>
{
    public SolutionState(
        ISystemState state,
        IEnumerable<IMaterial> materials,
        ISinteringConditions conditions
    )
    {
        Time = state.Time;
        Id = state.Id;
        Materials = materials.ToDictionary(m => m.Id);
        Particles = state
            .Particles.Select(ps => new Particle(ps, this, conditions))
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
    }

    private SolutionState(SolutionState oldState, StepVector stepVector, double timeStepWidth)
    {
        var newParticles = new Dictionary<Guid, Particle>()
        {
            [oldState.Particles.Root.Id] = oldState.Particles.Root.ApplyTimeStep(
                null,
                this,
                stepVector,
                timeStepWidth
            )
        };

        foreach (var contact in oldState.ParticleContacts)
        {
            newParticles[contact.To.Id] = contact.To.ApplyTimeStep(
                newParticles[contact.From.Id],
                this,
                stepVector,
                timeStepWidth
            );
        }

        Particles = newParticles.Values.ToReadOnlyParticleCollection<Particle, NodeBase>();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
        Materials = oldState.Materials;

        NodeContacts = oldState
            .NodeContacts.Select(c => new Edge<ContactNodeBase>(
                (ContactNodeBase)Nodes[c.From.Id],
                (ContactNodeBase)Nodes[c.To.Id],
                true
            ))
            .ToReadOnlyContactCollection();
        ParticleContacts = oldState
            .ParticleContacts.Select(c => new ParticleContact(
                Particles[c.From.Id],
                Particles[c.To.Id]
            ))
            .ToReadOnlyContactCollection();
    }

    public IReadOnlyDictionary<Guid, IMaterial> Materials { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISystemState.Nodes"/>>
    public IReadOnlyNodeCollection<NodeBase> Nodes { get; private set; }

    /// <inheritdoc cref="ISystemState.Particles"/>>
    public IReadOnlyParticleCollection<Particle, NodeBase> Particles { get; private set; }

    public IReadOnlyContactCollection<ParticleContact> ParticleContacts { get; private set; }

    public IReadOnlyContactCollection<IEdge<ContactNodeBase>> NodeContacts { get; private set; }

    IReadOnlyNodeCollection<NodeBase> IParticleSystem<Particle, NodeBase>.Nodes => Nodes;

    IReadOnlyParticleCollection<Particle, NodeBase> IParticleSystem<Particle, NodeBase>.Particles =>
        Particles;

    IReadOnlyContactCollection<IParticleContactEdge<Particle>> IParticleSystem<
        Particle,
        NodeBase
    >.ParticleContacts => ParticleContacts;

    IReadOnlyContactCollection<IEdge<NodeBase>> IParticleSystem<Particle, NodeBase>.NodeContacts =>
        NodeContacts;

    public SolutionState ApplyTimeStep(StepVector stepVector, double timeStepWidth) =>
        new(this, stepVector, timeStepWidth);

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
