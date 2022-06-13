using System.Collections.Generic;
using IMF.Enumerables;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.Solver.Solution;

namespace RefraSin.Core.Solver
{
    /// <summary>
    /// Interface for objects holding session data of a solution procedure.
    /// </summary>
    public interface ISinteringSolverSession
    {
        /// <summary>
        /// Time of the current step.
        /// </summary>
        public double CurrentTime { get; }
        
        /// <summary>
        /// Time where the solution started.
        /// </summary>
        public double StartTime { get; }
        
        /// <summary>
        /// Time where the solution should end.
        /// </summary>
        public double EndTime { get; }
        
        /// <summary>
        /// Current step width of time integration.
        /// </summary>
        public double TimeStepWidth { get; }
        
        /// <summary>
        /// Step width used in the last time step.
        /// </summary>
        public double? TimeStepWidthOfLastStep { get; }
        
        /// <summary>
        /// Tree of particles.
        /// </summary>
        public Tree<Particle> Particles { get; }
        
        /// <summary>
        /// Options for the solver.
        /// </summary>
        public SinteringSolverOptions SolverOptions { get; }
        
        /// <summary>
        /// Times series of solution states calculated so far.
        /// </summary>
        public IReadOnlyList<TimeSeriesItem> TimeSeries { get; }
    }
}