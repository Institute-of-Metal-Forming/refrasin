using System;
using IMF.Iteration;
using RefraSin.Coordinates;

namespace RefraSin.Core.Solver
{
    /// <summary>
    /// Class holding options to customize solver behavior.
    /// </summary>
    public class SinteringSolverOptions
    {
        /// <summary>
        /// Maximum count of iterations until an <see cref="IterationInterceptedException"/> is thrown.
        /// </summary>
        public int MaxIterationCount { get; set; } = 100;

        /// <summary>
        /// Goal precision of iteration loops as fraction of 1.
        /// </summary>
        public double IterationPrecision { get; set; } = 0.01;

        /// <summary>
        /// Goal discretization width in space.
        /// </summary>
        public double DiscretizationWidth { get; set; } = 10e-3;

        /// <summary>
        /// Count of time steps until an increase of time step width is tried again after decrease.
        /// </summary>
        public uint TimeStepIncreaseDelay { get; set; } = 3;

        /// <summary>
        /// Minimal valid time step width. Solution is canceled if time step width falls below.
        /// </summary>
        public double MinTimeStepWidth { get; set; } = 1e-8;

        /// <summary>
        /// Maximal valid time step width. Time step width is not further increased over this bound.
        /// </summary>
        public double MaxTimeStepWidth { get; set; } = Double.PositiveInfinity;

        /// <summary>
        /// Multiplicative factor of increase and decrease of time step width.
        /// </summary>
        public double TimeStepAdaptationFactor { get; set; } = 2.0;

        /// <summary>
        /// Gets or sets the maximum angle difference that is allowed at the surface-radius-angles in one time step.
        /// </summary>
        public Angle MaxSurfaceDisplacementAngle { get; set; } = Angle.FromDegrees(5);

        /// <summary>
        /// Gets or sets a values indicating whether grain boundary nodes are added along the grain boundary while remeshing.
        /// </summary>
        public bool AddAdditionalGrainBoundaryNodes { get; set; } = true;

        /// <summary>
        /// Fraction of <see cref="DiscretizationWidth"/> where nodes are deleted if deceeded.
        /// </summary>
        public double RemeshingDistanceDeletionLimit { get; set; } = 0.5;

        /// <summary>
        /// Fraction of <see cref="DiscretizationWidth"/> where nodes are inserted if exceeded.
        /// </summary>
        public double RemeshingDistanceInsertionLimit { get; set; } = 1.6;

        /// <summary>
        /// Fraction of the normal limits for remeshing to scale in grain boundaries.
        /// </summary>
        public double GrainBoundaryRemeshingInsertionRatio { get; set; } = 1.0;

        /// <summary>
        /// Time step width in first time step.
        /// </summary>
        public double InitialTimeStepWidth { get; set; } = 1;

        /// <summary>
        /// Brake factor in calculation of grain boundaries for the influence of neighbor nodes.
        /// </summary>
        public double GrainBoundaryStressChangeNeighborInfluenceBreakFactor { get; set; } = 0.1;

        /// <summary>
        /// Time out in seconds to cancel solution procedure after. Negative values disable this feature. 
        /// </summary>
        public double TimeOut { get; set; } = -1;
    }
}