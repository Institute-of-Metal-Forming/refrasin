using RefraSin.Numerics.LinearSolvers;
using RefraSin.Numerics.RootFinding;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.TEPSolver.BreakConditions;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.Recovery;
using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepEstimators;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepWidthControllers;
using RefraSin.TEPSolver.TimeSteppers;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.TEPSolver;

public record SolverRoutines(
    IStepEstimator StepEstimator,
    ITimeStepper TimeStepper,
    IEnumerable<IStepValidator> StepValidators,
    ILagrangianRootFinder LagrangianRootFinder,
    INormalizer Normalizer,
    IStepWidthController StepWidthController,
    IEnumerable<IStateRecoverer> StateRecoverers,
    IEnumerable<IBreakCondition> BreakConditions,
    IEnumerable<IParticleSystemRemesher> Remeshers,
    IEquationSystemBuilder EquationSystemBuilder
) : ISolverRoutines
{
    public static readonly SolverRoutines Default = new(
        new StepEstimator(),
        new EulerTimeStepper(),
        [],
        new DirectLagrangianRootFinder(
            new NewtonRaphsonRootFinder(new SparseLUSolver(), absoluteTolerance: 1e-4)
        ),
        new DefaultNormalizer(),
        new MaximumDisplacementAngleStepWidthController(),
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
        StepWidthController.RegisterWithSolver(solver);

        foreach (var validator in StepValidators)
            validator.RegisterWithSolver(solver);
        foreach (var recoverer in StateRecoverers)
            recoverer.RegisterWithSolver(solver);
    }
}
