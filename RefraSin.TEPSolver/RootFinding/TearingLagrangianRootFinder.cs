using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class TearingLagrangianRootFinder(IRootFinder particleBlockRootFinder, IRootFinder borderBlockRootFinder) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector initialGuess
    )
    {
        var stepVector = initialGuess.Copy();

        var solution = BorderBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.BorderBlock())
        );

        stepVector.UpdateBorderBlock(solution.AsArray());
        return stepVector;

        Vector<double> Fun(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = TearAndEvaluateFunctionalBlock(currentState, stepVector);
            return new DenseVector(result);
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = Jacobian.BorderBlock(currentState, stepVector);
            return result;
        }
    }

    private double[] TearAndEvaluateFunctionalBlock(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        foreach (var particle in currentState.Particles)
        {
            var particleSolution = SolveParticleBlock(particle, stepVector);
            stepVector.UpdateParticleBlock(particle, particleSolution.AsArray());
        }

        return Lagrangian
            .YieldFunctionalBlockEquations(currentState, stepVector)
            .ToArray();
    }

    private Vector<double> SolveParticleBlock(
        Particle particle,
        StepVector stepVector
    )
    {
        var solution = ParticleBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.ParticleBlock(particle))
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            stepVector.UpdateParticleBlock(particle, vector.AsArray());
            var result = Lagrangian.YieldParticleBlockEquations(particle, stepVector).ToArray();
            return new DenseVector(result);
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            stepVector.UpdateParticleBlock(particle, vector.AsArray());
            var result = Jacobian.ParticleBlock(particle, stepVector);
            return result;
        }
    }

    public IRootFinder ParticleBlockRootFinder { get; } = particleBlockRootFinder;

    public IRootFinder BorderBlockRootFinder { get; } = borderBlockRootFinder;
}