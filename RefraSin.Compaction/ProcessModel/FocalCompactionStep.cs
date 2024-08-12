using MoreLinq.Extensions;
using RefraSin.Compaction.ParticleModel;
using RefraSin.Coordinates;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ProcessModel;
using static RefraSin.ParticleModel.Nodes.NodeType;
using Node = RefraSin.Compaction.ParticleModel.Node;

namespace RefraSin.Compaction.ProcessModel;

public class FocalCompactionStep(IPoint focusPoint, double stepDistance, double minimumRelativeIntrusion = 0.2, int maxStepCount = 100)
    : ProcessStepBase
{

    /// <inheritdoc />
    public override ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    )
    {
        var bodies = inputState.Particles.Select(p => (IMoveableParticle)new Particle(p)).ToList();

        for (int i = 0; i < MaxStepCount; i++)
        {
            var combinationPairs = BuildCombinationPairs(bodies);

            foreach (var pair in combinationPairs)
            {
                if (pair[0].HasContactTo(pair[1], -MinimumIntrusion))
                {
                    bodies.Remove(pair[0]);
                    bodies.Remove(pair[1]);

                    var particles0 = pair[0].FlattenIfAgglomerate().ToArray();
                    var particles1 = pair[1].FlattenIfAgglomerate().ToArray();

                    foreach (var (p0, p1) in particles0.Cartesian(particles1, (p0, p1) => (p0, p1)))
                    {
                        if (p0.HasContactTo(p1, -MinimumIntrusion))
                        {
                            p0.CreateGrainBoundariesAtIntersections(
                                p1,
                                (point, particle) =>
                                    new Node(Guid.NewGuid(), point.Absolute, Neck, particle),
                                (point, particle) =>
                                    new Node(
                                        Guid.NewGuid(),
                                        point.Absolute,
                                        GrainBoundary,
                                        particle
                                    )
                            );
                        }
                    }
                    
                    bodies.Add(new ParticleAgglomerate(Guid.NewGuid(), particles0.Concat(particles1)));
                }
            }
            
            if (bodies.Count <= 1)
                break;
            
            foreach (var body in bodies)
            {
                body.MoveTowards(FocusPoint, StepDistance);
            }
        }

        return new SystemState(
            Guid.NewGuid(),
            inputState.Time,
            bodies.FlattenAgglomerates().Select(b => new Particle<IParticleNode>(b, (n, p) => new ParticleNode(n, p)))
        );
    }

    private static TElement[][] BuildCombinationPairs<TElement>(IEnumerable<TElement> bodies) =>
        bodies.Combinations(2).Select(c => c.ToArray()).ToArray();

    public IPoint FocusPoint { get; } = focusPoint.Absolute;

    public int MaxStepCount { get; } = maxStepCount;

    public double StepDistance { get; } = stepDistance;
    
    public double MinimumRelativeIntrusion { get; } = minimumRelativeIntrusion;

    public double MinimumIntrusion { get; } = stepDistance * minimumRelativeIntrusion;
}
