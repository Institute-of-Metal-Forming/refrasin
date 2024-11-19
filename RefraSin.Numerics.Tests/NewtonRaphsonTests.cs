using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using MathNet.Numerics.LinearAlgebra.Solvers;
using RefraSin.Numerics.LinearSolvers;
using RefraSin.Numerics.RootFinding;
using static System.Math;

namespace RefraSin.Numerics.Tests;

public class NewtonRaphsonTests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void XYOriginDistance()
    {
        Vector<double> Function(Vector<double> x)
        {
            return new DenseVector(
                [2 * x[0] + x[2] * x[1], 2 * x[1] + x[2] * x[0], x[0] * x[1] - 1]
            );
        }

        Matrix<double> Jacobian(Vector<double> x)
        {
            return Matrix<double>.Build.DenseOfArray(
                new[,]
                {
                    { 2, x[2], x[1] },
                    { x[2], 2, x[0] },
                    { x[1], x[0], 0 },
                }
            );
        }

        foreach (
            var linSolver in new ILinearSolver[]
            {
                new IterativeSolver(
                    new MlkBiCgStab(),
                    new Iterator<double>(),
                    new UnitPreconditioner<double>()
                ),
                new LUSolver(),
            }
        )
        {
            var solver = new NewtonRaphsonRootFinder(linSolver);
            var solution = solver.FindRoot(Function, Jacobian, new DenseVector([2.0, 2.0, 1.0]));

            Assert.That(solution.AsArray(), Is.EqualTo(new[] { 1.0, 1.0, -2.0 }).Within(1e-8));
        }
    }
}
