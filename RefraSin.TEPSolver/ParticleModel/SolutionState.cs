using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles.Extensions;
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

        ParticleContacts = Particles
            .EnumerateContactedParticlePairs<Particle, NodeBase>()
            .ToArray();
        NodeContacts = Nodes.EnumerateContactNodePairs().ToArray();
    }

    private SolutionState(SolutionState oldState, StepVector stepVector, double timeStepWidth)
    {
        Id = Guid.NewGuid();
        Time = oldState.Time + timeStepWidth;
        Materials = oldState.Materials;

        Particles = oldState
            .Particles.Select(p => p.ApplyTimeStep(this, stepVector, timeStepWidth))
            .ToReadOnlyParticleCollection<Particle, NodeBase>();

        Nodes = Particles.SelectMany(p => p.Nodes).ToReadOnlyNodeCollection();
        ParticleContacts = oldState
            .ParticleContacts.Select(c => new UnorderedPair<Particle>(
                Particles[c.First.Id],
                Particles[c.Second.Id]
            ))
            .ToArray();
        NodeContacts = oldState
            .NodeContacts.Select(c => new UnorderedPair<NodeBase>(
                Nodes[c.First.Id],
                Nodes[c.Second.Id]
            ))
            .ToArray();
    }

    public IReadOnlyDictionary<Guid, IMaterial> Materials { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISystemState.Nodes"/>>
    public IReadOnlyNodeCollection<NodeBase> Nodes { get; }

    /// <inheritdoc cref="ISystemState.Particles"/>>
    public IReadOnlyParticleCollection<Particle, NodeBase> Particles { get; }

    public IReadOnlyList<UnorderedPair<NodeBase>> NodeContacts { get; }

    public IReadOnlyList<UnorderedPair<Particle>> ParticleContacts { get; }

    IReadOnlyNodeCollection<NodeBase> IParticleSystem<Particle, NodeBase>.Nodes => Nodes;

    IReadOnlyParticleCollection<Particle, NodeBase> IParticleSystem<Particle, NodeBase>.Particles =>
        Particles;

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
