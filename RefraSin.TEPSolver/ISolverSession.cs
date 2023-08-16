using Microsoft.Extensions.Logging;
using RefraSin.Core.Solver;
using RefraSin.ParticleModel;
using RefraSin.Storage;

namespace RefraSin.TEPSolver
{
    /// <summary>
    /// Interface for objects holding session data of a solution procedure.
    /// </summary>
    public interface ISolverSession
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
        /// Registry of all particles.
        /// </summary>
        public IReadOnlyDictionary<Guid, IParticle> Particles { get; }

        /// <summary>
        /// Registry of all nodes of all particles.
        /// </summary>
        public IReadOnlyDictionary<Guid, INode> Nodes { get; }

        /// <summary>
        /// Options for the solver.
        /// </summary>
        public SolverOptions Options { get; }

        /// <summary>
        /// Object for storing the calculated solution data. Store and forget from the perspective of the solver.
        /// </summary>
        public ISolutionStorage SolutionStorage { get; }

        /// <summary>
        /// Factory for loggers used in the session.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }
    }
}