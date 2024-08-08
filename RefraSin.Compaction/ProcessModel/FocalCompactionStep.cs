using RefraSin.Compaction.ParticleModel;
using RefraSin.Coordinates;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using Node = RefraSin.ParticleModel.Nodes.Node;

namespace RefraSin.Compaction.ProcessModel;

public class FocalCompactionStep : ProcessStepBase
{
    public FocalCompactionStep(IPoint focusPoint)
    {
        FocusPoint = focusPoint.Absolute;
    }

    /// <inheritdoc />
    public override ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    )
    {
        var bodies = inputState.Particles.Select(p => new Particle(p)).ToList();

        for (int i = 0; i < MaxStepCount; i++)
        {
            foreach (var body in bodies)
            {
                body.MoveTowards(FocusPoint, StepDistance);
            }

            foreach (var body in bodies) { }
        }

        return new SystemState(
            Guid.NewGuid(),
            inputState.Time,
            bodies.Select(b => new Particle<IParticleNode>(b, (n, p) => new ParticleNode(n, p)))
        );
    }

    public IPoint FocusPoint { get; }

    public int MaxStepCount { get; }

    public double StepDistance { get; }
}
