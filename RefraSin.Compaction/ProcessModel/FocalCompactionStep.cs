using MoreLinq.Extensions;
using RefraSin.Compaction.ParticleModel;
using RefraSin.Coordinates;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ProcessModel;
using static RefraSin.ParticleModel.Nodes.NodeType;
using Node = RefraSin.Compaction.ParticleModel.Node;

namespace RefraSin.Compaction.ProcessModel;

public class FocalCompactionStep(
    IPoint focusPoint,
    double stepDistance,
    double minimumRelativeIntrusion = 0.2,
    int maxStepCount = 100
) : ProcessStepBase
{
    /// <inheritdoc />
    public override ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    )
    {
        var bodies = new LinkedList<IMoveableParticle>(
            inputState.Particles.Select(p => new Particle(p))
        );

        for (int i = 0; i < MaxStepCount; i++)
        {
            var current = bodies.First;

            while (current != null)
            {
                var other = current.Next;

                while (other != null)
                {
                    if (current.Value.IntersectsWith(other.Value, -MinimumIntrusion))
                    {
                        var particles0 = current.Value.FlattenIfAgglomerate().ToArray();
                        var particles1 = other.Value.FlattenIfAgglomerate().ToArray();

                        foreach (
                            var (p0, p1) in particles0.Cartesian(particles1, (p0, p1) => (p0, p1))
                        )
                        {
                            if (p0.IntersectsWith(p1, -1e-2 * MinimumIntrusion))
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

                        current.Value = new ParticleAgglomerate(
                            Guid.NewGuid(),
                            particles0.Concat(particles1)
                        );
                        var next = other.Next;
                        bodies.Remove(other);
                        other = next;
                    }
                    else
                    {
                        other = other.Next;
                    }
                }

                current = current.Next;
            }

            if (bodies.Count <= 1)
                break;

            foreach (var body in bodies)
            {
                body.MoveTowards(FocusPoint, StepDistance);
            }
        }

        var particles = bodies
            .FlattenAgglomerates()
            .Select(b => new Particle<ParticleNode>(b, (n, p) => new ParticleNode(n, p)))
            .ToReadOnlyParticleCollection<Particle<ParticleNode>, ParticleNode>();

        return new SystemState(
            Guid.NewGuid(),
            inputState.Time,
            inputState.Particles.Select(pOld => particles[pOld.Id])
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
