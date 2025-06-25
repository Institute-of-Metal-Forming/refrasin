using RefraSin.Compaction.ParticleModel;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ProcessModel;
using static RefraSin.ParticleModel.Nodes.NodeType;
using Node = RefraSin.Compaction.ParticleModel.Node;

namespace RefraSin.Compaction.ProcessModel;

public class OneByOneCompactionStep(
    double stepDistance,
    double minimumIntrusion,
    int maxStepCount = 100
) : ProcessStepBase
{
    public int MaxStepCount { get; } = maxStepCount;

    public double StepDistance { get; } = stepDistance;

    public double MinimumIntrusion { get; } =  minimumIntrusion;

    public override ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    )
    {
        var particleEnumerator = inputState.Particles.Select(p => new Particle(p)).GetEnumerator();

        if (!particleEnumerator.MoveNext())
            throw new InvalidOperationException("state is empty");

        var immobileParticles = new List<IMoveableParticle> { particleEnumerator.Current };

        while (particleEnumerator.MoveNext())
        {
            var mobileParticle = particleEnumerator.Current;

            for (int i = 0; i < MaxStepCount; i++)
            {
                var contacts = immobileParticles
                    .Select(p =>
                        (p: p, intersects: mobileParticle.IntersectsWith(p, -MinimumIntrusion))
                    )
                    .ToArray();

                if (contacts.Count(t => t.intersects) >= 2)
                    break;

                var vector = contacts
                    .Select(t =>
                        mobileParticle.Coordinates.VectorTo(t.p.Coordinates)
                        * (t.intersects ? -1 : 1)
                    )
                    .Aggregate(new AbsoluteVector(), (v, sum) => sum.Add(v));

                mobileParticle.MoveBy(vector, StepDistance);
                ReportSystemState(
                    new SystemState(
                        Guid.NewGuid(),
                        inputState.Time,
                        immobileParticles
                            .Append(mobileParticle)
                            .Select(p =>
                                Particle<ParticleNode>.FromTemplate(
                                    p,
                                    (n, particle) => new ParticleNode(n, particle)
                                )
                            )
                    )
                );
            }

            foreach (var p in immobileParticles)
            {
                mobileParticle.CreateGrainBoundariesAtIntersections(
                    p,
                    (point, particle) => new Node(Guid.NewGuid(), point.Absolute, Neck, particle),
                    (point, particle) =>
                        new Node(Guid.NewGuid(), point.Absolute, GrainBoundary, particle)
                );
            }

            immobileParticles.Add(mobileParticle);
        }

        particleEnumerator.Dispose();
        return new SystemState(
            Guid.NewGuid(),
            inputState.Time,
            immobileParticles.Select(p =>
                Particle<ParticleNode>.FromTemplate(
                    p,
                    (n, particle) => new ParticleNode(n, particle)
                )
            )
        );
    }
}
