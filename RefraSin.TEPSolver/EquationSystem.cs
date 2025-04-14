using MathNet.Numerics.LinearAlgebra;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

public class EquationSystem
{
    internal EquationSystem(
        SolutionState solutionState,
        IEnumerable<IQuantity> quantities,
        IEnumerable<IConstraint> constraints
    )
    {
        State = solutionState;
        Constraints = constraints.ToArray();
        Quantities = quantities.ToArray();
    }

    public SolutionState State { get; }

    public IReadOnlyList<IQuantity> Quantities { get; }
    public IReadOnlyList<IConstraint> Constraints { get; }

    public Vector<double> Lagrangian(StepVector stepVector)
    {
        var constraintDerivatives = Constraints
            .SelectMany(c => c.Derivatives(stepVector).Select(d => (c, d.index, d.value)))
            .GroupBy(t => t.index, t => (t.c, t.value))
            .ToDictionary(g => g.Key, g => g.ToArray());

        var quantityDerivatives = Quantities.Select(q =>
            (q is IStateVelocity sv ? sv.DrivingForce(stepVector) : 0)
            + constraintDerivatives
                .GetValueOrDefault(stepVector.StepVectorMap.QuantityIndex(q), [])
                .Sum(t => t.value * stepVector.ConstraintLambdaValue(t.c))
        );

        var constraintResiduals = Constraints.Select(c => c.Residual(stepVector));

        return Vector<double>.Build.DenseOfEnumerable(
            quantityDerivatives.Concat(constraintResiduals)
        );
    }

    public Matrix<double> Jacobian(StepVector stepVector)
    {
        var constraintComponents = Constraints.SelectMany(c =>
        {
            var rowIndex = stepVector.StepVectorMap.ConstraintIndex(c);
            return c.Derivatives(stepVector).Select(d => (rowIndex, columnIndex: d.index, d.value));
        });

        var secondDerivatives = Constraints
            .SelectMany(c =>
            {
                var lambdaIndex = stepVector.StepVectorMap.ConstraintIndex(c);
                return c.SecondDerivatives(stepVector)
                    .Select(d => (lambdaIndex, d.firstIndex, d.secondIndex, d.value));
            })
            .GroupBy(t => (t.firstIndex, t.secondIndex), t => (t.lambdaIndex, t.value))
            .Select(g =>
                (
                    g.Key.firstIndex,
                    g.Key.secondIndex,
                    g.Sum(e => e.value * stepVector[e.lambdaIndex])
                )
            );

        return Matrix<double>.Build.SparseOfIndexed(
            stepVector.Count,
            stepVector.Count,
            constraintComponents
                .Concat(secondDerivatives)
                .SelectMany(SelectJacobianComponentsSymmetrically)
        );

        static IEnumerable<(
            int rowIndex,
            int columnIndex,
            double value
        )> SelectJacobianComponentsSymmetrically((int rowIndex, int columnIndex, double value) c)
        {
            yield return c;
            if (c.columnIndex != c.rowIndex)
                yield return (c.columnIndex, c.rowIndex, c.value);
        }
    }
}
