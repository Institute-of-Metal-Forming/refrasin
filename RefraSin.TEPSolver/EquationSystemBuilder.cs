using System.Runtime.InteropServices.ComTypes;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;

namespace RefraSin.TEPSolver;

public class EquationSystemBuilder
{
    private readonly List<(
        Func<SolutionState, bool> condition,
        Func<SolutionState, ISystemItem> ctor
    )> _globalItems = new();

    private readonly List<(
        Func<Particle, bool> condition,
        Func<Particle, ISystemItem> ctor
    )> _particleItems = new();

    private readonly List<(
        Func<NodeBase, bool> condition,
        Func<NodeBase, ISystemItem> ctor
    )> _nodeItems = new();

    private readonly List<(
        Func<ContactPair<NodeBase>, bool> condition,
        Func<ContactPair<NodeBase>, ISystemItem> ctor
    )> _nodeContactItems = new();

    private readonly List<(
        Func<ContactPair<Particle>, bool> condition,
        Func<ContactPair<Particle>, ISystemItem> ctor
    )> _particleContactItems = new();

    public EquationSystemBuilder AddGlobalItem<TItem>(Func<SolutionState, bool>? condition = null)
        where TItem : IGlobalItem
    {
        _globalItems.Add((condition ?? (_ => true), TItem.Create));
        return this;
    }

    public EquationSystemBuilder AddParticleItem<TItem>(Func<Particle, bool>? condition = null)
        where TItem : IParticleItem
    {
        _particleItems.Add((condition ?? (_ => true), TItem.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeItem<TItem>(Func<NodeBase, bool>? condition = null)
        where TItem : INodeItem
    {
        _nodeItems.Add((condition ?? (_ => true), TItem.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeItem<TItem, TNode>()
        where TItem : INodeItem
        where TNode : NodeBase
    {
        _nodeItems.Add((n => n is TNode, TItem.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeContactItem<TItem>(
        Func<ContactPair<NodeBase>, bool>? condition = null
    )
        where TItem : INodeContactItem
    {
        _nodeContactItems.Add((condition ?? (_ => true), TItem.Create));
        return this;
    }

    public EquationSystemBuilder AddParticleContactItem<TItem>(
        Func<ContactPair<Particle>, bool>? condition = null
    )
        where TItem : IParticleContactItem
    {
        _particleContactItems.Add((condition ?? (_ => true), TItem.Create));
        return this;
    }

    public EquationSystem Build(SolutionState solutionState)
    {
        return new EquationSystem(solutionState, CreateSystemItems(solutionState));
    }

    private IEnumerable<ISystemItem> CreateSystemItems(SolutionState solutionState)
    {
        foreach (var (condition, item) in _globalItems)
        {
            if (condition(solutionState))
                yield return item(solutionState);
        }

        foreach (var particle in solutionState.Particles)
        {
            foreach (var (condition, item) in _particleItems)
            {
                if (condition(particle))
                    yield return item(particle);
            }

            foreach (var node in particle.Nodes)
            foreach (var (condition, item) in _nodeItems)
            {
                if (condition(node))
                    yield return item(node);
            }
        }

        foreach (var particleContact in solutionState.ParticleContacts)
        {
            foreach (var (condition, item) in _particleContactItems)
            {
                if (condition(particleContact))
                    yield return item(particleContact);
            }
        }

        foreach (var nodeContact in solutionState.NodeContacts)
        {
            foreach (var (condition, item) in _nodeContactItems)
            {
                if (condition(nodeContact))
                    yield return item(nodeContact);
            }
        }
    }

    public static EquationSystemBuilder Default =>
        new EquationSystemBuilder()
            .AddNodeItem<NormalDisplacement>()
            .AddNodeItem<TangentialDisplacement>(n => n.Type is NodeType.Neck)
            .AddNodeItem<FluxToUpper>()
            .AddParticleItem<ParticleDisplacementX>()
            .AddParticleItem<ParticleDisplacementY>()
            .AddGlobalItem<DissipationEqualityConstraint>()
            .AddNodeItem<VolumeBalanceConstraint>()
            .AddNodeContactItem<ContactConstraintX>()
            .AddNodeContactItem<ContactConstraintY>()
            .AddParticleItem<FixedParticleConstraintX>(particle =>
                particle.SolutionState.Particles[0] == particle
            )
            .AddParticleItem<FixedParticleConstraintY>(particle =>
                particle.SolutionState.Particles[0] == particle
            );
}
