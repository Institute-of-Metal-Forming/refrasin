using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.ProcessModel;

namespace RefraSin.TEPSolver.Test;

public static class InitialStates
{
    public static IEnumerable<(
        string label,
        ISystemState<IParticle<IParticleNode>, IParticleNode> state
    )> Generate()
    {
        yield return (nameof(OneParticle), OneParticle());
        yield return (nameof(Symmetric3PointBoundary), Symmetric3PointBoundary());
        // yield return (nameof(Symmetric5PointBoundary), Symmetric5PointBoundary());
        yield return (nameof(FourParticleRing), FourParticleRing());
        yield return (nameof(ThreeParticleRingCircular), ThreeParticleRingCircular());
        yield return (nameof(ThreeParticleTreeCircular), ThreeParticleTreeCircular());
    }

    public static readonly Guid MaterialId = Guid.NewGuid();

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> OneParticle()
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (0, 0),
            0,
            nodeCountPerParticle,
            100e-6,
            0.2,
            5,
            0.2
        ).GetParticle();

        return new SystemState(Guid.Empty, 0, [particle1]);
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> Symmetric3PointBoundary()
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (0, 0),
            0,
            nodeCountPerParticle,
            100e-6,
            0.2,
            5,
            0.2
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (300e-6, 0),
            Angle.Half,
            nodeCountPerParticle,
            100e-6,
            0.2,
            5,
            0.2
        ).GetParticle();

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2]);
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 2e-6).Solve(
            initialState
        );

        return compactedState;
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> Symmetric5PointBoundary()
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (0, 0),
            0,
            nodeCountPerParticle,
            100e-6,
            0.2,
            5,
            0.2
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (300e-6, 0),
            0,
            nodeCountPerParticle,
            100e-6,
            0.2,
            5,
            0.2
        ).GetParticle();

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2]);
        var compactedState = new FocalCompactionStep(
            new AbsolutePoint(0, 0),
            8e-6,
            minimumRelativeIntrusion: 0.9
        ).Solve(initialState);

        var remeshedState = new GrainBoundaryRemesher(-0.1).RemeshSystem(compactedState);

        return new SystemState(Guid.NewGuid(), compactedState.Time, remeshedState);
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> FourParticleRing()
    {
        var nodeCountPerParticle = 40;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (-130e-6, -130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (-130e-6, 130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var particle3 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (130e-6, -130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var particle4 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (130e-6, 130e-6),
            0,
            nodeCountPerParticle,
            100e-6,
            0,
            4,
            0.2
        ).GetParticle();

        var initialState = new SystemState(
            Guid.Empty,
            0,
            [particle1, particle2, particle3, particle4]
        );
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 2e-6).Solve(
            initialState
        );

        return compactedState;
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> ThreeParticleRingCircular()
    {
        var nodeCountPerParticle = 40;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (0, -110e-6),
            0,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (105e-6, -110e-6),
            0,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var particle3 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (-105e-6, -110e-6),
            0,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2, particle3]);
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 5e-6).Solve(
            initialState
        );

        return compactedState;
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> ThreeParticleTreeCircular()
    {
        var nodeCountPerParticle = 40;

        var particle1 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (0, 0),
            0,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var particle2 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (150 - 6, 150e-6),
            0,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var particle3 = new ShapeFunctionParticleFactoryCosOvalityCosPeaks(
            MaterialId,
            (-150e-6, 150e-6),
            0,
            nodeCountPerParticle,
            100e-6
        ).GetParticle();

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2, particle3]);
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 2e-6).Solve(
            initialState
        );

        return compactedState;
    }
}
