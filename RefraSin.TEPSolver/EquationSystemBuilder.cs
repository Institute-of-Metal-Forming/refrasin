using System.Runtime.InteropServices.ComTypes;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;

namespace RefraSin.TEPSolver;

public class EquationSystemBuilder
{
    private readonly List<Func<SolutionState, IQuantity>> _globalQuantities = new();

    private readonly List<Func<SolutionState, Particle, IQuantity>> _particleQuantities = new();

    private readonly List<(
        Func<NodeBase, bool> condition,
        Func<SolutionState, NodeBase, IQuantity> ctor
    )> _nodeQuantities = new();

    public EquationSystemBuilder AddGlobalQuantity<TQuantity>()
        where TQuantity : IGlobalQuantity
    {
        _globalQuantities.Add(TQuantity.Create);
        return this;
    }

    public EquationSystemBuilder AddParticleQuantity<TQuantity>()
        where TQuantity : IParticleQuantity
    {
        _particleQuantities.Add(TQuantity.Create);
        return this;
    }

    public EquationSystemBuilder AddNodeQuantity<TQuantity>()
        where TQuantity : INodeQuantity
    {
        _nodeQuantities.Add((_ => true, TQuantity.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeQuantity<TQuantity, TNode>()
        where TQuantity : INodeQuantity
        where TNode : NodeBase
    {
        _nodeQuantities.Add((n => n is TNode, TQuantity.Create));
        return this;
    }

    private readonly List<Func<SolutionState, IConstraint>> _globalConstraints = new();
    private readonly List<Func<SolutionState, Particle, IConstraint>> _particleConstraints = new();

    private readonly List<(
        Func<NodeBase, bool> condition,
        Func<SolutionState, NodeBase, IConstraint> ctor
    )> _nodeConstraints = new();

    public EquationSystemBuilder AddGlobalConstraint<TConstraint>()
        where TConstraint : IGlobalConstraint
    {
        _globalConstraints.Add(TConstraint.Create);
        return this;
    }

    public EquationSystemBuilder AddParticleConstraint<TConstraint>()
        where TConstraint : IParticleConstraint
    {
        _particleConstraints.Add(TConstraint.Create);
        return this;
    }

    public EquationSystemBuilder AddNodeConstraint<TConstraint>()
        where TConstraint : INodeConstraint
    {
        _nodeConstraints.Add((_ => true, TConstraint.Create));
        return this;
    }

    public EquationSystemBuilder AddNodeConstraint<TConstraint, TNode>()
        where TConstraint : INodeConstraint
        where TNode : NodeBase
    {
        _nodeConstraints.Add((n => n is TNode, TConstraint.Create));
        return this;
    }

    public EquationSystem Build(SolutionState solutionState) =>
        new(solutionState, CreateQuantities(solutionState), CreateConstraints(solutionState));

    private IEnumerable<IQuantity> CreateQuantities(SolutionState solutionState)
    {
        foreach (var globalQuantity in _globalQuantities)
        {
            yield return globalQuantity(solutionState);
        }

        foreach (var particle in solutionState.Particles)
        {
            foreach (var particleQuantity in _particleQuantities)
            {
                yield return particleQuantity(solutionState, particle);
            }

            foreach (var node in particle.Nodes)
            foreach (var (condition, nodeQuantity) in _nodeQuantities)
            {
                if (condition(node))
                    yield return nodeQuantity(solutionState, node);
            }
        }
    }

    private IEnumerable<IConstraint> CreateConstraints(SolutionState solutionState)
    {
        foreach (var globalConstraint in _globalConstraints)
        {
            yield return globalConstraint(solutionState);
        }

        foreach (var particle in solutionState.Particles)
        {
            foreach (var particleConstraint in _particleConstraints)
            {
                yield return particleConstraint(solutionState, particle);
            }

            foreach (var node in particle.Nodes)
            foreach (var (condtion, nodeConstraint) in _nodeConstraints)
            {
                if (condtion(node))
                    yield return nodeConstraint(solutionState, node);
            }
        }
    }
}
