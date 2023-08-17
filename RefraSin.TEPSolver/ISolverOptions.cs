using RefraSin.Coordinates;
using RefraSin.Iteration;

namespace RefraSin.TEPSolver;

/// <summary>
/// Interface for types providing options to the solver.
/// </summary>
public interface ISolverOptions
{
    /// <summary>
    /// Maximum count of iterations until an <see cref="IterationInterceptedException"/> is thrown.
    /// </summary>
    int MaxIterationCount { get; }

    /// <summary>
    /// Goal precision of iteration loops as fraction of 1.
    /// </summary>
    double IterationPrecision { get; }

    /// <summary>
    /// Goal discretization width in space.
    /// </summary>
    double DiscretizationWidth { get; }

    /// <summary>
    /// Count of time steps until an increase of time step width is tried again after decrease.
    /// </summary>
    uint TimeStepIncreaseDelay { get; }

    /// <summary>
    /// Minimal valid time step width. Solution is canceled if time step width falls below.
    /// </summary>
    double MinTimeStepWidth { get; }

    /// <summary>
    /// Maximal valid time step width. Time step width is not further increased over this bound.
    /// </summary>
    double MaxTimeStepWidth { get; }

    /// <summary>
    /// Multiplicative factor of increase and decrease of time step width.
    /// </summary>
    double TimeStepAdaptationFactor { get; }

    /// <summary>
    /// Gets or sets the maximum angle difference that is allowed at the surface-radius-angles in one time step.
    /// </summary>
    Angle MaxSurfaceDisplacementAngle { get; }

    /// <summary>
    /// Gets or sets a values indicating whether grain boundary nodes are added along the grain boundary while remeshing.
    /// </summary>
    bool AddAdditionalGrainBoundaryNodes { get; }

    /// <summary>
    /// Fraction of <see cref="DiscretizationWidth"/> where nodes are deleted if deceeded.
    /// </summary>
    double RemeshingDistanceDeletionLimit { get; }

    /// <summary>
    /// Fraction of <see cref="DiscretizationWidth"/> where nodes are inserted if exceeded.
    /// </summary>
    double RemeshingDistanceInsertionLimit { get; }

    /// <summary>
    /// Fraction of the normal limits for remeshing to scale in grain boundaries.
    /// </summary>
    double GrainBoundaryRemeshingInsertionRatio { get; }

    /// <summary>
    /// Time step width in first time step.
    /// </summary>
    double InitialTimeStepWidth { get; }

    /// <summary>
    /// Time out in seconds to cancel solution procedure after. Negative values disable this feature.
    /// </summary>
    double TimeOut { get; }
}