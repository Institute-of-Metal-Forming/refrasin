using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.Storage;
using Node = RefraSin.TEPSolver.ParticleModel.Node;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

public partial class Solver
{
    private class SolverSession : ISolverSession
    {
        /// <inheritdoc />
        public double CurrentTime { get; }

        /// <inheritdoc />
        public double StartTime { get; }

        /// <inheritdoc />
        public double EndTime { get; }

        /// <inheritdoc />
        public double TimeStepWidth { get; }

        /// <inheritdoc />
        public double? TimeStepWidthOfLastStep { get; }

        /// <inheritdoc />
        public Particle RootParticle { get; }

        /// <inheritdoc cref="ISolverSession.Particles"/>
        public Dictionary<Guid, Particle> Particles { get; }

        IReadOnlyDictionary<Guid, Particle> ISolverSession.Particles => Particles;

        /// <inheritdoc cref="ISolverSession.Nodes"/>
        public Dictionary<Guid, Node> Nodes { get; }

        IReadOnlyDictionary<Guid, Node> ISolverSession.Nodes => Nodes;

        /// <inheritdoc />
        public ISolverOptions Options { get; }

        /// <inheritdoc />
        public ISolutionStorage SolutionStorage { get; }

        /// <inheritdoc />
        public IReadOnlyMaterialRegistry MaterialRegistry { get; }

        /// <inheritdoc />
        public ILogger<Solver> Logger { get; }
    }
}