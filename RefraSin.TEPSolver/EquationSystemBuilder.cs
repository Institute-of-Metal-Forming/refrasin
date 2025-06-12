using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;

namespace RefraSin.TEPSolver;

public class EquationSystemBuilder : IEquationSystemBuilder
{
    public EquationSystemBuilder ConfigureAdditionalItems(
        Func<SolutionState, IEnumerable<ISystemItem>> builderFunction
    )
    {
        _additionalItemsBuilders.Add(builderFunction);
        return this;
    }

    private readonly List<Func<SolutionState, IEnumerable<ISystemItem>>> _additionalItemsBuilders =
        new();

    public EquationSystem Build(SolutionState solutionState)
    {
        return new EquationSystem(solutionState, CreateSystemItems(solutionState));
    }

    private IEnumerable<ISystemItem> CreateSystemItems(SolutionState solutionState)
    {
        yield return new DissipationEqualityConstraint(solutionState);

        yield return new FixedParticleConstraintX(solutionState.Particles[0]);
        yield return new FixedParticleConstraintY(solutionState.Particles[0]);

        foreach (var particle in solutionState.Particles)
        {
            yield return new ParticleDisplacementX(particle);
            yield return new ParticleDisplacementY(particle);

            foreach (var node in particle.Nodes)
            {
                yield return new NormalDisplacement(node);

                if (node.Type is NodeType.Neck)
                    yield return new TangentialDisplacement(node);

                yield return new FluxToUpper(node);
                yield return new VolumeBalanceConstraint(node);
            }
        }

        foreach (var nodeContact in solutionState.NodeContacts)
        {
            yield return new ContactConstraintX(nodeContact);
            yield return new ContactConstraintY(nodeContact);
        }

        foreach (var builder in _additionalItemsBuilders)
        foreach (var item in builder(solutionState))
            yield return item;
    }
}
