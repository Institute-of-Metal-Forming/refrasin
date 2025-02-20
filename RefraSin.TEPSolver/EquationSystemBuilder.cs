using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;

namespace RefraSin.TEPSolver;

public class EquationSystemBuilder
{
    private readonly List<Func<SolutionState, IQuantity>> _globalQuantities = new();

    private readonly List<Func<SolutionState, Particle, IQuantity>> _particleQuantities = new();

    private readonly List<Func<SolutionState, NodeBase, IQuantity>> _nodeQuantities = new();

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
        _nodeQuantities.Add(TQuantity.Create);
        return this;
    }

    private readonly List<Func<SolutionState, IConstraint>> _globalConstraints = new();
    private readonly List<Func<SolutionState, Particle, IConstraint>> _particleConstraints = new();
    private readonly List<Func<SolutionState, NodeBase, IConstraint>> _nodeConstraints = new();

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
        _nodeConstraints.Add(TConstraint.Create);
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
            foreach (var nodeQuantity in _nodeQuantities)
            {
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
            foreach (var nodeConstraint in _nodeConstraints)
            {
                yield return nodeConstraint(solutionState, node);
            }
        }
    }
}
