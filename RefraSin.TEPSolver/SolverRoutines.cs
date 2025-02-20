using RefraSin.Numerics.LinearSolvers;
using RefraSin.Numerics.RootFinding;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.Quantities;
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
    IEnumerable<IParticleSystemRemesher> Remeshers,
    EquationSystemBuilder EquationSystemBuilder
) : ISolverRoutines
{
    static SolverRoutines()
    {
        var equationSystemBuilder = new EquationSystemBuilder();

        equationSystemBuilder.AddNodeQuantity<NormalDisplacement>();
        equationSystemBuilder.AddNodeQuantity<TangentialDisplacement>();
        equationSystemBuilder.AddNodeQuantity<FluxToUpper>();

        equationSystemBuilder.AddParticleQuantity<ParticleDisplacementX>();
        equationSystemBuilder.AddParticleQuantity<ParticleDisplacementY>();

        equationSystemBuilder.AddGlobalConstraint<DissipationEqualityConstraint>();
        equationSystemBuilder.AddNodeConstraint<VolumeBalanceConstraint>();
        equationSystemBuilder.AddNodeConstraint<ContactConstraintX>();
        equationSystemBuilder.AddNodeConstraint<ContactConstraintY>();

        Default = new(
            new StepEstimator(),
            new AdamsMoultonTimeStepper(),
            [],
            new DirectLagrangianRootFinder(
                new NewtonRaphsonRootFinder(new SparseLUSolver(), absoluteTolerance: 1e-4)
            ),
            new DefaultNormalizer(),
            new MaximumDisplacementAngleStepWidthController(),
            [new StepBackStateRecoverer()],
            [new FreeSurfaceRemesher(), new NeckNeighborhoodRemesher()],
            equationSystemBuilder
        );
    }

    public static readonly SolverRoutines Default;

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
