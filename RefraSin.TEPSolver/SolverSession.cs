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
        public SolverSession(Solver solver, ISolutionState initialState, double endTime)
        {
            EndTime = endTime;
            CurrentTime = initialState.Time;
            StartTime = CurrentTime;
            TimeStepWidth = solver.Options.InitialTimeStepWidth;
            TimeStepWidthOfLastStep = TimeStepWidth;
            Options = solver.Options;
            SolutionStorage = solver.SolutionStorage;
            MaterialRegistry = solver.MaterialRegistry;
            Logger = solver.LoggerFactory.CreateLogger<Solver>();

            var particles = initialState.ParticleStates.Select(ps => new Particle(null, ps, this)).ToArray();
            RootParticle = particles[0];
            Particles = particles.ToDictionary(p => p.Id);
            Nodes = particles.SelectMany(p => p.Surface).ToDictionary(n => n.Id);
        }

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