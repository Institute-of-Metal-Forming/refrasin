using System.Collections.Generic;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.Solver.Solution
{
    /// <summary>
    /// Interface for classes holding information about results of sintering solution procedure.
    /// </summary>
    public interface ISinteringSolverSolution
    {
        /// <summary>
        /// Indicates if the solution procedure was successful.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Instance of the process that was solved.
        /// </summary>
        public ISinteringProcess Process { get; }

        /// <summary>
        /// List of the time series states of the solution.
        /// </summary>
        public IReadOnlyList<TimeSeriesItem> TimeSeries { get; }
    }
}