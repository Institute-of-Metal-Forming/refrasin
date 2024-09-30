using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using RefraSin.Numerics.Exceptions;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class TearingLagrangianRootFinder(
    IRootFinder particleBlockRootFinder,
    IRootFinder contactBlockRootFinder,
    IRootFinder globalBlockRootFinder,
    double iterationPrecision = 1e-4,
    int maxIterationCount = 100
) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(SolutionState currentState, StepVector initialGuess, ILogger logger)
    {
        var stepVector = initialGuess.Copy();
        var system = new EquationSystem.EquationSystem(currentState, stepVector);

        TearAndSolveBlocks(system, logger);
        return stepVector;
    }

    private void TearAndSolveBlocks(EquationSystem.EquationSystem system, ILogger logger)
    {
        int i;

        for (i = 0; i < MaxIterationCount; i++)
        {
            var oldVector = system.StepVector.Copy();

            foreach (var particle in system.State.Particles)
            {
                var particleSolution = SolveParticleBlock(system, particle, logger);
                system.StepVector.UpdateParticleBlock(particle, particleSolution.AsArray());
            }

            foreach (var contact in system.State.ParticleContacts)
            {
                var contactSolution = SolveContactBlock(system, contact, logger);
                system.StepVector.UpdateContactBlock(contact, contactSolution.AsArray());
            }

            var globalSolution = SolveGlobalBlock(system, logger);
            system.StepVector.UpdateGlobalBlock(globalSolution.AsArray());

            if ((system.StepVector - oldVector).L2Norm() < IterationPrecision * oldVector.L2Norm())
            {
                logger.LogInformation("{Loop} iterations: {Count}", nameof(TearAndSolveBlocks), i);
                return;
            }
        }

        throw new UncriticalIterationInterceptedException(
            $"{nameof(TearingLagrangianRootFinder)}.{nameof(FindRoot)}",
            InterceptReason.MaxIterationCountExceeded,
            i
        );
    }

    private Vector<double> SolveContactBlock(
        EquationSystem.EquationSystem system,
        ParticleContact contact,
        ILogger logger
    )
    {
        var solution = ContactBlockRootFinder.FindRoot(
            Fun,
            Jac,
            system.StepVector.ContactBlock(contact),
            logger
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            system.StepVector.UpdateContactBlock(contact, vector.AsArray());
            var result = system.ContactBlockLagrangian(contact);
            return result;
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            system.StepVector.UpdateContactBlock(contact, vector.AsArray());
            var result = system.ContactBlockJacobian(contact);
            return result;
        }
    }

    private Vector<double> SolveParticleBlock(
        EquationSystem.EquationSystem system,
        Particle particle,
        ILogger logger
    )
    {
        var solution = ParticleBlockRootFinder.FindRoot(
            Fun,
            Jac,
            system.StepVector.ParticleBlock(particle),
            logger
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            system.StepVector.UpdateParticleBlock(particle, vector.AsArray());
            var result = system.ParticleBlockLagrangian(particle);
            return result;
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            system.StepVector.UpdateParticleBlock(particle, vector.AsArray());
            var result = system.ParticleBlockJacobian(particle);
            return result;
        }
    }

    private Vector<double> SolveGlobalBlock(EquationSystem.EquationSystem system, ILogger logger)
    {
        var solution = GlobalBlockRootFinder.FindRoot(
            Fun,
            Jac,
            system.StepVector.GlobalBlock(),
            logger
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            system.StepVector.UpdateGlobalBlock(vector.AsArray());
            var result = system.GlobalBlockLagrangian();
            return result;
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            system.StepVector.UpdateGlobalBlock(vector.AsArray());
            var result = system.GlobalBlockJacobian();
            return result;
        }
    }

    public IRootFinder ParticleBlockRootFinder { get; } = particleBlockRootFinder;

    public IRootFinder ContactBlockRootFinder { get; } = contactBlockRootFinder;

    public IRootFinder GlobalBlockRootFinder { get; } = globalBlockRootFinder;

    public int MaxIterationCount { get; } = maxIterationCount;

    public double IterationPrecision { get; } = iterationPrecision;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
