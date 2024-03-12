using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using MathNet.Numerics.LinearAlgebra.Solvers;
using RefraSin.Numerics.LinearSolvers;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepEstimators;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.TimeSteppers;

namespace RefraSin.TEPSolver;

public record SolverRoutines(
    IStepEstimator StepEstimator,
    ITimeStepper TimeStepper,
    IEnumerable<IStepValidator> StepValidators,
    ILagrangianRootFinder LagrangianRootFinder
) : ISolverRoutines
{
    public static SolverRoutines Default = new(
        new StepEstimator(),
        new AdamsMoultonTimeStepper(),
        new[]
        {
            new InstabilityDetector()
        },
        new TearingLagrangianRootFinder(
            new NewtonRaphsonRootFinder(new IterativeSolver(
                    new MlkBiCgStab(),
                    new Iterator<double>(),
                    new MILU0Preconditioner()
                )
            ),
            new NewtonRaphsonRootFinder(
                new LUSolver()
            )
        )
    );
}