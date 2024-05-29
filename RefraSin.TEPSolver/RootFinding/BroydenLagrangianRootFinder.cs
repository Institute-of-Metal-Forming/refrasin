using MathNet.Numerics.RootFinding;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class BroydenLagrangianRootFinder : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector initialGuess
    )
    {
        double[] Fun(double[] vector)
        {
            var result = Lagrangian.EvaluateAt(currentState, new StepVector(vector, initialGuess.StepVectorMap)).AsArray();
            return result;
        }

        var result = Broyden.FindRoot(
            Fun,
            initialGuess: initialGuess.AsArray(),
            maxIterations: solverSession.Options.RootFindingMaxIterationCount,
            accuracy: solverSession.Options.RootFindingAccuracy
        );

        return new StepVector(result, initialGuess.StepVectorMap);
    }
}