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

public interface ISolverRoutines
{
    /// <summary>
    /// Subroutine for estimating a time step as initial value.
    /// </summary>
    IStepEstimator StepEstimator { get; }

    /// <summary>
    /// Subroutine to calculate a time step.
    /// </summary>
    ITimeStepper TimeStepper { get; }

    /// <summary>
    /// Collection of routines to validate each calculated time step.
    /// </summary>
    IEnumerable<IStepValidator> StepValidators { get; }

    /// <summary>
    /// Subroutine to calculate the root of the Lagrangian gradient.
    /// </summary>
    ILagrangianRootFinder LagrangianRootFinder { get; }

    /// <summary>
    /// Subroutine to normalize and denormalize states for solution and reporting.
    /// </summary>
    INormalizer Normalizer { get; }

    /// <summary>
    /// Subroutines to determine time step width. Minimal suggested value is actually taken.
    /// </summary>
    IEnumerable<IStepWidthController> StepWidthControllers { get; }

    /// <summary>
    /// Collection of routines for recovering invalid solution states.
    /// </summary>
    IEnumerable<IStateRecoverer> StateRecoverers { get; }

    /// <summary>
    /// Collection of routines for checking if simulation shall be aborted.
    /// </summary>
    IEnumerable<IBreakCondition> BreakConditions { get; }

    /// <summary>
    /// Collection of routines for recovering invalid solution states.
    /// </summary>
    IEnumerable<IParticleSystemRemesher> Remeshers { get; }

    /// <summary>
    /// Routine to construct equation system.
    /// </summary>
    IEquationSystemBuilder EquationSystemBuilder { get; }

    void RegisterWithSolver(SinteringSolver solver);
}
