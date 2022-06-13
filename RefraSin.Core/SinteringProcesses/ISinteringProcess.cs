using System.Threading;
using System.Threading.Tasks;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleTreeCompactors;
using RefraSin.Core.ParticleTreeSources;
using RefraSin.Core.Solver.Solution;

namespace RefraSin.Core.SinteringProcesses
{
    /// <summary>
    /// Interface for all sintering processes.
    /// </summary>
    public interface ISinteringProcess
    {
        /// <summary>
        /// Label to identify this process.
        /// </summary>
        public string Label { get; }

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
        /// Start time of solving.
        /// </summary>
        double StartTime { get; }

        /// <summary>
        /// End time of solving. Values smaller then <see cref="StartTime"/> lead to aborting the solving procedure after generating the initial state.
        /// </summary>
        double EndTime { get; }

        /// <summary>
        /// Solve the process.
        /// </summary>
        /// <param name="cancellationToken"></param>
        ISinteringSolverSolution Solve(CancellationToken cancellationToken);

        /// <summary>
        /// Solve the process asyncronously.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<ISinteringSolverSolution> SolveAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// The universal gas constant R.
        /// </summary>
        public double UniversalGasConstant { get; }
        
        /// <summary>
        /// The temperature at which this process is performed.
        /// </summary>
        public double Temperature { get; }
    }
}