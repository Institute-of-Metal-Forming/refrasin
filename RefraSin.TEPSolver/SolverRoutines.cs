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
            new NewtonRaphsonRootFinder()
        )
    );
}