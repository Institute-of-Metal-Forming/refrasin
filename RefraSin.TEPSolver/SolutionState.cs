using RefraSin.ParticleModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.ParticleModel;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

public class SolutionState : ISolutionState
{
    public SolutionState(double time, IReadOnlyParticleCollection<Particle> particles)
    {
        Time = time;
        Particles = particles;
        AllNodes = particles.SelectMany(p => p.Nodes).ToDictionaryById();
    }

    /// <inheritdoc />
    public double Time { get; }

    public IReadOnlyParticleCollection<Particle> Particles { get; }

    public IReadOnlyDictionary<Guid, NodeBase> AllNodes { get; }

    /// <inheritdoc />
    IReadOnlyList<IParticle> ISolutionState.ParticleStates => Particles;
}