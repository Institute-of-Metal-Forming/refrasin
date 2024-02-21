using MathNet.Numerics.LinearAlgebra;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public static class Helper
{
    public static IEnumerable<T> Join<T>(params IEnumerable<T>[] equations) =>
        equations.SelectMany(e => e);

    public static Vector<double> ToBorderRowVector(this IEnumerable<(int colIndex, double value)> indexedValues, StepVector stepVector) =>
        Vector<double>.Build.SparseOfIndexed(stepVector.StepVectorMap.BorderLength, indexedValues);
}