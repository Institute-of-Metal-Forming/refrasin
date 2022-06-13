using RefraSin.Core.Solver;

namespace RefraSin.Core.ParticleTreeCompactors
{
    /// <summary>
    /// Interface for classes, that provide an algorithm for compaction of particle trees (approaching the particles until contact).
    /// </summary>
    public interface IParticleTreeCompactor
    {
        /// <summary>
        /// Execute compaction on a specified particle tree.
        /// </summary>
        /// <param name="session"></param>
        public void Compact(ISinteringSolverSession session);
    }
}