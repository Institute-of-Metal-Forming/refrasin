using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class ParticleDisplacementXDerivative : ParticleEquationBase
{
    /// <inheritdoc />
    public ParticleDisplacementXDerivative(SolutionState state, Particle particle, StepVector step)
        : base(state, particle, step) { }

    /// <inheritdoc />
    public override double Value() =>
        Particle
            .Nodes.OfType<ContactNodeBase>()
            .Sum(n =>
                Step.LambdaContactX(n)
                * State.NodeContacts.FromOrTo(n.Id).Single().IfVertexIs(n, () => 1, () => -1)
            );

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative() { }
}
