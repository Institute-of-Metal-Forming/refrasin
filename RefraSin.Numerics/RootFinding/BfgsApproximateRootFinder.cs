using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;

namespace RefraSin.Numerics.RootFinding;

public class BfgsApproximateRootFinder(
    double gradientTolerance = 1e-8,
    double parameterTolerance = 1e-8,
    double functionProgressTolerance = 1e-8
) : IRootFinder
{
    /// <inheritdoc />
    public Vector<double> FindRoot(
        Func<Vector<double>, Vector<double>> function,
        Func<Vector<double>, Matrix<double>> jacobian,
        Vector<double> initialGuess
    )
    {
        double Fun(Vector<double> x)
        {
            var result = function(x);

            return result * result;
        }

        Vector<double> Grad(Vector<double> x)
        {
            var functionResult = function(x);
            var jacobianResult = jacobian(x);

            return jacobianResult * functionResult + functionResult * jacobianResult;
        }

        var objFunction = ObjectiveFunction.Gradient(Fun, Grad);
        var minimizer = new BfgsMinimizer(
            GradientTolerance,
            ParameterTolerance,
            FunctionProgressTolerance
        );

        var result = minimizer.FindMinimum(objFunction, initialGuess);

        return result.MinimizingPoint;
    }

    public double GradientTolerance { get; } = gradientTolerance;
    public double ParameterTolerance { get; } = parameterTolerance;
    public double FunctionProgressTolerance { get; } = functionProgressTolerance;
}
