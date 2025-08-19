using System.Text;
using RefraSin.Compaction.ParticleModel;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Particles.Extensions;
using RefraSin.ProcessModel;
using static RefraSin.ParticleModel.Nodes.NodeType;
using Log = Serilog.Log;
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

    public double MinimumIntrusion { get; } = minimumIntrusion;

    public override ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    )
    {
        var logger = Log.ForContext<OneByOneCompactionStep>();
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

                if (contacts.Count(t => t.intersects) >= (immobileParticles.Count > 1 ? 2 : 1))
                {
                    logger.Information(
                        "Contact created with {Count}/{Required} particles.",
                        contacts.Count(t => t.intersects),
                        (immobileParticles.Count > 1 ? 2 : 1)
                    );
                    break;
                }

                var vectors = contacts
                    .Select(t =>
                        mobileParticle.Coordinates.VectorTo(t.p.Coordinates)
                        * (t.intersects ? -0.99 : 1)
                    )
                    .ToArray();
                var vector = vectors.Aggregate(new AbsoluteVector(), (v, sum) => sum.Add(v));

                logger.Debug(
                    "Move along {Vector} (Components {Components}).",
                    vector,
                    new StringBuilder().AppendJoin(", ", vectors)
                );
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
                logger.Information(
                    "Creating grain boundaries between {Mobile} and {Immobile}.",
                    mobileParticle,
                    p
                );
                mobileParticle.CreateGrainBoundariesAtIntersections(
                    p,
                    (point, particle) => new Node(Guid.NewGuid(), point.Absolute, Neck, particle),
                    (point, particle) =>
                        new Node(Guid.NewGuid(), point.Absolute, GrainBoundary, particle)
                );
            }

            immobileParticles.Add(mobileParticle);
        }

        logger.Information("All particles processed.");

        particleEnumerator.Dispose();
        var resultState = new SystemState(
            Guid.NewGuid(),
            inputState.Time,
            immobileParticles.Select(p =>
                Particle<ParticleNode>.FromTemplate(
                    p,
                    (n, particle) => new ParticleNode(n, particle)
                )
            )
        );
        ReportSystemState(resultState);
        return resultState;
    }
}
