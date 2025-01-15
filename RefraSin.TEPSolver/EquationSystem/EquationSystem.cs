using MathNet.Numerics.LinearAlgebra;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class EquationSystem
{
    public EquationSystem(SolutionState solutionState, StepVector stepVector)
    {
        State = solutionState;
        StepVector = stepVector;
        Equations = YieldEquations().ToArray();
    }

    public SolutionState State { get; }

    public StepVector StepVector { get; }

    private IEnumerable<IEquation> YieldEquations()
    {
        foreach (var particle in State.Particles)
        {
            foreach (var n in particle.Nodes)
            {
                yield return new NormalDisplacementDerivative(State, n, StepVector);
                if (n is NeckNode)
                    yield return new TangentialDisplacementDerivative(State, n, StepVector);
                yield return new FluxDerivative(State, n, StepVector);
                yield return new VolumeBalanceConstraint(State, n, StepVector);
            }

            yield return new ParticleDisplacementXDerivative(State, particle, StepVector);
            yield return new ParticleDisplacementYDerivative(State, particle, StepVector);
        }

        foreach (var nodeContact in State.NodeContacts)
        {
            yield return new ContactConstraintY(State, nodeContact.From, StepVector);
            yield return new ContactConstraintX(State, nodeContact.From, StepVector);
        }

        yield return new DissipationEqualityConstraint(State, StepVector);
    }

    public IReadOnlyList<IEquation> Equations { get; }

    public Vector<double> Lagrangian() =>
        Vector<double>.Build.DenseOfEnumerable(Equations.Select(e => e.Value()));

    public Matrix<double> Jacobian() =>
        Matrix<double>.Build.SparseOfIndexed(
            StepVector.Count,
            StepVector.Count,
            Equations.SelectMany((e, i) => e.Derivative().Select(c => (i, c.Item1, c.Item2)))
        );
}
