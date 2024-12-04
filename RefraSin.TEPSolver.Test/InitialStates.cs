using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
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
    }

    public static readonly Guid MaterialId = Guid.NewGuid();

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> OneParticle()
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, MaterialId)
        {
            NodeCount = nodeCountPerParticle,
        }.GetParticle();

        return new SystemState(Guid.Empty, 0, [particle1]);
    }

    private static ISystemState<IParticle<IParticleNode>, IParticleNode> Symmetric3PointBoundary()
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, MaterialId)
        {
            NodeCount = nodeCountPerParticle,
        }.GetParticle();

        var particle2 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, MaterialId)
        {
            NodeCount = nodeCountPerParticle,
            RotationAngle = Angle.Half,
            CenterCoordinates = (300e-6, 0),
        }.GetParticle();

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2]);
        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 2e-6).Solve(
            initialState
        );

        return compactedState;
    }
}
