using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.System;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

public class SolutionState : ISystemState<Particle, NodeBase>
{
    public SolutionState(
        ISystemState state,
        IEnumerable<IParticleMaterial> materials,
        ISinteringConditions conditions
    )
    {
        Time = state.Time;
        Id = state.Id;
        Materials = materials.ToDictionary(m => m.Id);
        Particles = state
            .Particles.Select(ps => new Particle(ps, this, conditions))
            .ToReadOnlyVertexCollection();
        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyVertexCollection();

        NodeContacts = Nodes.CreateContactNodePairs().ToArray();
        ParticleContacts = NodeContacts
            .DistinctBy(c => (c.First.ParticleId, c.Second.ParticleId))
            .Select(c => new ContactPair<Particle>(
                Guid.NewGuid(),
                Particles[c.First.ParticleId],
                Particles[c.Second.ParticleId]
            ))
            .ToArray();
    }

    private SolutionState(SolutionState oldState, StepVector stepVector, double timeStepWidth)
    {
        Id = Guid.NewGuid();
        Time = oldState.Time + timeStepWidth;
        Materials = oldState.Materials;

        Particles = oldState
            .Particles.Select(p => p.ApplyTimeStep(this, stepVector, timeStepWidth))
            .ToReadOnlyVertexCollection();

        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyVertexCollection();
        ParticleContacts = oldState
            .ParticleContacts.Select(c => new ContactPair<Particle>(
                c.Id,
                Particles[c.First.Id],
                Particles[c.Second.Id]
            ))
            .ToArray();
        NodeContacts = oldState
            .NodeContacts.Select(c => new ContactPair<NodeBase>(
                c.Id,
                Nodes[c.First.Id],
                Nodes[c.Second.Id]
            ))
            .ToArray();
    }

    public IReadOnlyDictionary<Guid, IParticleMaterial> Materials { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISystemState.Nodes"/>>
    public IReadOnlyVertexCollection<NodeBase> Nodes { get; }

    /// <inheritdoc cref="ISystemState.Particles"/>>
    public IReadOnlyVertexCollection<Particle> Particles { get; }

    public IReadOnlyList<ContactPair<NodeBase>> NodeContacts { get; }

    public IReadOnlyList<ContactPair<Particle>> ParticleContacts { get; }

    IReadOnlyVertexCollection<NodeBase> IParticleSystem<Particle, NodeBase>.Nodes => Nodes;

    IReadOnlyVertexCollection<Particle> IParticleSystem<Particle, NodeBase>.Particles => Particles;

    public SolutionState ApplyTimeStep(StepVector stepVector, double timeStepWidth) =>
        new(this, stepVector, timeStepWidth);

    public void Sanitize()
    {
        var newNodeCoordinates = NodeContacts
            .Select(e =>
            {
                var node = Nodes[e.First.Id];
                var contactedNode = Nodes[e.Second.Id];
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
