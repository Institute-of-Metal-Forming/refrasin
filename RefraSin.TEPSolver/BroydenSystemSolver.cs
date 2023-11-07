using MathNet.Numerics.RootFinding;
using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver;

public class BroydenSystemSolver : ISystemSolver
{
    /// <inheritdoc />
    public StepVector Solve(ISolverSession solverSession, StepVector initialGuess)
    {
        double[] Fun(double[] vector) =>
            LagrangianGradient.EvaluateAt(solverSession, new StepVector(vector, initialGuess.StepVectorMap)).AsArray();

        return new StepVector(Broyden.FindRoot(
            Fun,
            initialGuess: initialGuess.AsArray(),
            maxIterations: solverSession.Options.RootFindingMaxIterationCount,
            accuracy: solverSession.Options.RootFindingAccuracy
        ), initialGuess.StepVectorMap);
    }
}