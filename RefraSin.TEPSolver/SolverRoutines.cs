using RefraSin.Numerics.LinearSolvers;
using RefraSin.Numerics.RootFinding;
using RefraSin.ParticleModel.Remeshing;
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
    IStepWidthController StepWidthController,
    IEnumerable<IStateRecoverer> StateRecoverers,
    IEnumerable<IParticleSystemRemesher> Remeshers) : ISolverRoutines
{
    public static readonly SolverRoutines Default = new(
        new StepEstimator(),
        new AdamsMoultonTimeStepper(),
        [
            // new InstabilityDetector()
        ],
        new TearingLagrangianRootFinder(
            new NewtonRaphsonRootFinder(
                new LUSolver(),
                absoluteTolerance: 1e-4
            ),
            new NewtonRaphsonRootFinder(
                new LUSolver(),
                absoluteTolerance: 1e-4
            )
        ),
        new DefaultNormalizer(),
        new MaximumDisplacementAngleStepWidthController(),
        [
            new StepBackStateRecoverer()
        ],
        [
            new FreeSurfaceRemesher(),
            new NeckNeighborhoodRemesher(),
            // new GrainBoundaryRemesher(),
        ]
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