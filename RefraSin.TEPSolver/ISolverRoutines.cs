using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.TimeSteppers;

namespace RefraSin.TEPSolver;

public interface ISolverRoutines
{
    /// <summary>
    /// Subroutine for evaluating the Lagrangian gradient and its Jacobian.
    /// </summary>
    ILagrangianGradient LagrangianGradient { get; }
    
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
    IRootFinder RootFinder { get; }
}