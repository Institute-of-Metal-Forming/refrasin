using System.Collections.Generic;
using System.Linq;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.Solver.Solution
{
    /// <summary>
    /// Class holding information about results of sintering solution procedure.
    /// </summary>
    public class SolutionResult : ISinteringSolverSolution
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="succeeded">whether the procedure succeeded</param>
        /// <param name="timeSeries">sequence of the time series states of the solution</param>
        /// <param name="process">the process instance that was solved</param>
        public SolutionResult(bool succeeded, IEnumerable<TimeSeriesItem> timeSeries, ISinteringProcess process)
        {
            Succeeded = succeeded;
            TimeSeries = timeSeries.ToList();
            Process = process;
        }

        /// <inheritdoc />
        public bool Succeeded { get; }

        /// <inheritdoc />
        public ISinteringProcess Process { get; }

        /// <inheritdoc />
        public IReadOnlyList<TimeSeriesItem> TimeSeries { get; }
    }
}