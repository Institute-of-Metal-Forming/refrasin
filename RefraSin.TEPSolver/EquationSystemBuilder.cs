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
        Func<SolutionState, IQuantity> ctor
    )> _globalQuantities = new();

    private readonly List<(
        Func<Particle, bool> condition,
        Func<Particle, IQuantity> ctor
    )> _particleQuantities = new();

    private readonly List<(
        Func<NodeBase, bool> condition,
        Func<NodeBase, IQuantity> ctor
    )> _nodeQuantities = new();

    private readonly List<(
        Func<ContactPair<NodeBase>, bool> condition,
        Func<ContactPair<NodeBase>, IQuantity> ctor
    )> _nodeContactQuantities = new();

    private readonly List<(
        Func<ContactPair<Particle>, bool> condition,
        Func<ContactPair<Particle>, IQuantity> ctor
    )> _particleContactQuantities = new();

    public EquationSystemBuilder AddGlobalQuantity<TQuantity>(
        Func<SolutionState, bool>? condition = null
    )
        where TQuantity : IGlobalQuantity
    {
        _globalQuantities.Add((condition ?? (_ => true), TQuantity.Create));
        return this;
    }

    public EquationSystemBuilder AddParticleQuantity<TQuantity>(
        Func<Particle, bool>? condition = null
    )
        where TQuantity : IParticleQuantity
    {
        _particleQuantities.Add((condition ?? (_ => true), TQuantity.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeQuantity<TQuantity>(Func<NodeBase, bool>? condition = null)
        where TQuantity : INodeQuantity
    {
        _nodeQuantities.Add((condition ?? (_ => true), TQuantity.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeQuantity<TQuantity, TNode>()
        where TQuantity : INodeQuantity
        where TNode : NodeBase
    {
        _nodeQuantities.Add((n => n is TNode, TQuantity.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeContactQuantity<TQuantity>(
        Func<ContactPair<NodeBase>, bool>? condition = null
    )
        where TQuantity : INodeContactQuantity
    {
        _nodeContactQuantities.Add((condition ?? (_ => true), TQuantity.Create));
        return this;
    }

    public EquationSystemBuilder AddParticleContactQuantity<TQuantity>(
        Func<ContactPair<Particle>, bool>? condition = null
    )
        where TQuantity : IParticleContactQuantity
    {
        _particleContactQuantities.Add((condition ?? (_ => true), TQuantity.Create));
        return this;
    }

    private readonly List<(
        Func<SolutionState, bool> condition,
        Func<SolutionState, IConstraint> ctor
    )> _globalConstraints = new();

    private readonly List<(
        Func<Particle, bool> condition,
        Func<Particle, IConstraint> ctor
    )> _particleConstraints = new();

    private readonly List<(
        Func<NodeBase, bool> condition,
        Func<NodeBase, IConstraint> ctor
    )> _nodeConstraints = new();

    private readonly List<(
        Func<ContactPair<NodeBase>, bool> condition,
        Func<ContactPair<NodeBase>, IConstraint> ctor
    )> _nodeContactConstraints = new();

    private readonly List<(
        Func<ContactPair<Particle>, bool> condition,
        Func<ContactPair<Particle>, IConstraint> ctor
    )> _particleContactConstraints = new();

    public EquationSystemBuilder AddGlobalConstraint<TConstraint>(
        Func<SolutionState, bool>? condition = null
    )
        where TConstraint : IGlobalConstraint
    {
        _globalConstraints.Add((condition ?? (_ => true), TConstraint.Create));
        return this;
    }

    public EquationSystemBuilder AddParticleConstraint<TConstraint>(
        Func<Particle, bool>? condition = null
    )
        where TConstraint : IParticleConstraint
    {
        _particleConstraints.Add((condition ?? (_ => true), TConstraint.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeConstraint<TConstraint>(
        Func<NodeBase, bool>? condition = null
    )
        where TConstraint : INodeConstraint
    {
        _nodeConstraints.Add((condition ?? (_ => true), TConstraint.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeConstraint<TConstraint, TNode>()
        where TConstraint : INodeConstraint
        where TNode : NodeBase
    {
        _nodeConstraints.Add((n => n is TNode, TConstraint.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeContactConstraint<TConstraint>(
        Func<ContactPair<NodeBase>, bool>? condition = null
    )
        where TConstraint : INodeContactConstraint
    {
        _nodeContactConstraints.Add((condition ?? (_ => true), TConstraint.Create));
        return this;
    }

    public EquationSystemBuilder AddParticleContactConstraint<TConstraint>(
        Func<ContactPair<Particle>, bool>? condition = null
    )
        where TConstraint : IParticleContactConstraint
    {
        _particleContactConstraints.Add((condition ?? (_ => true), TConstraint.Create));
        return this;
    }

    public EquationSystem Build(SolutionState solutionState) =>
        new(solutionState, CreateQuantities(solutionState), CreateConstraints(solutionState));

    private IEnumerable<IQuantity> CreateQuantities(SolutionState solutionState) =>
        CreateSystemItems(
            solutionState,
            _globalQuantities,
            _particleQuantities,
            _nodeQuantities,
            _particleContactQuantities,
            _nodeContactQuantities
        );

    private IEnumerable<IConstraint> CreateConstraints(SolutionState solutionState) =>
        CreateSystemItems(
            solutionState,
            _globalConstraints,
            _particleConstraints,
            _nodeConstraints,
            _particleContactConstraints,
            _nodeContactConstraints
        );

    private IEnumerable<T> CreateSystemItems<T>(
        SolutionState solutionState,
        IReadOnlyList<(
            Func<SolutionState, bool> condition,
            Func<SolutionState, T> ctor
        )> globalItems,
        IReadOnlyList<(Func<Particle, bool> condition, Func<Particle, T> ctor)> particleItems,
        IReadOnlyList<(Func<NodeBase, bool> condition, Func<NodeBase, T> ctor)> nodeItems,
        IReadOnlyList<(
            Func<ContactPair<Particle>, bool> condition,
            Func<ContactPair<Particle>, T> ctor
        )> particleContactItems,
        IReadOnlyList<(
            Func<ContactPair<NodeBase>, bool> condition,
            Func<ContactPair<NodeBase>, T> ctor
        )> nodeContactItems
    )
    {
        foreach (var (condition, item) in globalItems)
        {
            if (condition(solutionState))
                yield return item(solutionState);
        }

        foreach (var particle in solutionState.Particles)
        {
            foreach (var (condition, item) in particleItems)
            {
                if (condition(particle))
                    yield return item(particle);
            }

            foreach (var node in particle.Nodes)
            foreach (var (condition, item) in nodeItems)
            {
                if (condition(node))
                    yield return item(node);
            }
        }

        foreach (var particleContact in solutionState.ParticleContacts)
        {
            foreach (var (condition, item) in particleContactItems)
            {
                if (condition(particleContact))
                    yield return item(particleContact);
            }
        }

        foreach (var nodeContact in solutionState.NodeContacts)
        {
            foreach (var (condition, item) in nodeContactItems)
            {
                if (condition(nodeContact))
                    yield return item(nodeContact);
            }
        }
    }

    public static EquationSystemBuilder Default =>
        new EquationSystemBuilder()
            .AddNodeQuantity<NormalDisplacement>()
            .AddNodeQuantity<TangentialDisplacement>(n => n.Type is NodeType.Neck)
            .AddNodeQuantity<FluxToUpper>()
            .AddParticleQuantity<ParticleDisplacementX>()
            .AddParticleQuantity<ParticleDisplacementY>()
            .AddGlobalConstraint<DissipationEqualityConstraint>()
            .AddNodeConstraint<VolumeBalanceConstraint>()
            .AddNodeContactConstraint<ContactConstraintX>()
            .AddNodeContactConstraint<ContactConstraintY>()
            .AddParticleConstraint<FixedParticleConstraintX>(particle =>
                particle.SolutionState.Particles[0] == particle
            )
            .AddParticleConstraint<FixedParticleConstraintY>(particle =>
                particle.SolutionState.Particles[0] == particle
            );
}
