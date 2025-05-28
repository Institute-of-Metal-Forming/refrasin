using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Solvers;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

public class EquationSystem
{
    internal EquationSystem(SolutionState solutionState, IEnumerable<ISystemItem> items)
    {
        State = solutionState;
        Items = items.ToArray();
        _constraints = Items.OfType<IConstraint>().ToArray();
    }

    public SolutionState State { get; }

    public IReadOnlyList<ISystemItem> Items { get; }

    private IReadOnlyList<IConstraint> _constraints;

    public int Size => Items.Count;

    public double Dissipation(StepVector stepVector) =>
        Items
            .OfType<IStateVelocity>()
            .Sum(q => q.DrivingForce(stepVector) * stepVector.ItemValue(q));

    public Vector<double> Lagrangian(StepVector stepVector)
    {
        var constraintDerivatives = _constraints
            .SelectMany(c => c.Derivatives(this, stepVector).Select(d => (c, d.index, d.value)))
            .GroupBy(t => t.index, t => (t.c, t.value))
            .ToDictionary(g => g.Key, g => g.ToArray());

        return Vector<double>.Build.DenseOfEnumerable(
            Items.Select(i =>
                i switch
                {
                    IQuantity q => (q is IStateVelocity sv ? sv.DrivingForce(stepVector) : 0)
                        + constraintDerivatives
                            .GetValueOrDefault(stepVector.StepVectorMap.ItemIndex(q), [])
                            .Sum(t => t.value * stepVector.ItemValue(t.c)),
                    IConstraint c => c.Residual(this, stepVector),
                    _ => throw new InvalidOperationException(),
                }
            )
        );
    }

    public Matrix<double> Jacobian(StepVector stepVector)
    {
        var constraintComponents = _constraints.SelectMany(c =>
        {
            var rowIndex = stepVector.StepVectorMap.ItemIndex(c);
            return c.Derivatives(this, stepVector)
                .Select(d => (rowIndex, columnIndex: d.index, d.value));
        });

        var secondDerivatives = _constraints
            .SelectMany(c =>
            {
                var lambdaIndex = stepVector.StepVectorMap.ItemIndex(c);
                return c.SecondDerivatives(this, stepVector)
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
