using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleTreeCompactors;
using RefraSin.Core.ParticleTreeSources;
using RefraSin.Core.Solver;
using RefraSin.Core.Solver.Solution;

namespace RefraSin.Core.SinteringProcesses
{
    /// <summary>
    ///     Represents a sintering process of certain particles.
    /// </summary>
    public class SinteringProcess : ISinteringProcess
    {
        /// <summary>
        /// Creates a new sintering process.
        /// </summary>
        /// <param name="treeSource">particle tree source used to generate the particle object tree</param>
        /// <param name="startTime">start time of solving</param>
        /// <param name="endTime">end time of solving</param>
        /// <param name="materialInterfaces">sequence of possible material interfaces with their key values, all possible occuring interfaces must be specified</param>
        /// <param name="temperature">the temperature at which this process is performed</param>
        /// <param name="compactor"><see cref="IParticleTreeCompactor"/> implementation to used to compact the generated particle tree. If null, an instance of <see cref="OneDirectionalParticleTreeCompactor"/> will be used.</param>
        public SinteringProcess(IParticleTreeSource treeSource, double startTime, double endTime,
            IEnumerable<MaterialInterface> materialInterfaces, double temperature, IParticleTreeCompactor? compactor = null)
        {
            TreeSource = treeSource;
            Compactor = compactor ?? new OneDirectionalParticleTreeCompactor();
            StartTime = startTime;
            EndTime = endTime;
            Temperature = temperature;
            MaterialInterfaces = new MaterialInterfaceCollection(materialInterfaces);
        }

        /// <summary>
        ///     Start time of solving.
        /// </summary>
        public double StartTime { get; }

        /// <summary>
        ///     End time of solving. Values smaller then <see cref="StartTime" /> lead to aborting the solving procedure after
        ///     generating the initial state.
        /// </summary>
        public double EndTime { get; }

        /// <inheritdoc />
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Collection of possible material interfaces with their key values. All possible occuring interfaces must be specified.
        /// </summary>
        public MaterialInterfaceCollection MaterialInterfaces { get; }

        /// <summary>
        /// Particle tree source used to generate the particle object tree.
        /// </summary>
        public IParticleTreeSource TreeSource { get; }

        /// <summary>
        /// The <see cref="IParticleTreeCompactor"/> implementation to used to compact the generated particle tree
        /// </summary>
        public IParticleTreeCompactor Compactor { get; }

        /// <summary>
        /// Options for solver behavior.
        /// </summary>
        public SinteringSolverOptions SolverOptions { get; set; } = new();

        /// <summary>
        ///     Solve the process.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public ISinteringSolverSolution Solve(CancellationToken cancellationToken) => SinteringSolver.Solve(this, SolverOptions, cancellationToken);

        /// <summary>
        ///     Solve the process asyncronously.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<ISinteringSolverSolution> SolveAsync(CancellationToken cancellationToken) =>
            Task.Run(() => Solve(cancellationToken));
        
        /// <summary>
        /// The universal gas constant R (default in SI units = 8.31446261815324D).
        /// </summary>
        public double UniversalGasConstant { get; set; } = 8.31446261815324D;

        /// <inheritdoc />
        public double Temperature { get; }
    }
}