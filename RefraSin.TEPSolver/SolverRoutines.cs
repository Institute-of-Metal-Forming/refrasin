using RefraSin.Numerics.LinearSolvers;
using RefraSin.Numerics.RootFinding;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.TEPSolver.BreakConditions;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.Recovery;
using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepEstimators;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepWidthControllers;
using RefraSin.TEPSolver.TimeSteppers;

namespace RefraSin.TEPSolver;

public record SolverRoutines(
    IStepEstimator StepEstimator,
    ITimeStepper TimeStepper,
    IEnumerable<IStepValidator> StepValidators,
    ILagrangianRootFinder LagrangianRootFinder,
    INormalizer Normalizer,
    IEnumerable<IStepWidthController> StepWidthControllers,
    IEnumerable<IStateRecoverer> StateRecoverers,
    IEnumerable<IBreakCondition> BreakConditions,
    IEnumerable<IParticleSystemRemesher> Remeshers,
    IEquationSystemBuilder EquationSystemBuilder
) : ISolverRoutines
{
    public static readonly SolverRoutines Default = new(
        new RememberingStepEstimator(new StepEstimator()),
        new EulerTimeStepper(),
        [],
        new DirectLagrangianRootFinder(
            new NewtonRaphsonRootFinder(new SparseLUSolver(), absoluteTolerance: 1e-4)
        ),
        new DefaultNormalizer(),
        [new MaximumDisplacementAngleStepWidthController(), new PoreElasticStepWidthController()],
        [new StepBackStateRecoverer()],
        [],
        [new FreeSurfaceRemesher(), new NeckNeighborhoodRemesher()],
        new EquationSystemBuilder()
    );

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver)
    {
        StepEstimator.RegisterWithSolver(solver);
        TimeStepper.RegisterWithSolver(solver);
        LagrangianRootFinder.RegisterWithSolver(solver);
        Normalizer.RegisterWithSolver(solver);

        foreach (var controller in StepWidthControllers)
            controller.RegisterWithSolver(solver);
        foreach (var validator in StepValidators)
            validator.RegisterWithSolver(solver);
        foreach (var recoverer in StateRecoverers)
            recoverer.RegisterWithSolver(solver);
        foreach (var condition in BreakConditions)
            condition.RegisterWithSolver(solver);
    }
}
