using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.Test;

public static class InitialStates
{
    public static IEnumerable<(string label, SolutionState state)> Generate(
        ISinteringConditions conditions
    )
    {
        yield return (nameof(OneParticle), OneParticle(conditions));
        yield return (nameof(Symmetric3PointBoundary), Symmetric3PointBoundary(conditions));
    }

    private static SolutionState OneParticle(ISinteringConditions conditions)
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, Guid.NewGuid())
        {
            NodeCount = nodeCountPerParticle,
        }.GetParticle();

        var material = new Material(
            particle1.MaterialId,
            "Al2O3",
            new BulkProperties(0, 1e-4),
            new SubstanceProperties(1.8e3, 101.96e-3),
            new InterfaceProperties(1.65e-10, 0.9),
            new Dictionary<Guid, IInterfaceProperties>()
        );

        var initialState = new SystemState(Guid.Empty, 0, [particle1]);
        var norm = new DefaultNormalizer().GetNorm(initialState, conditions, [material]);
        var normalizedState = norm.NormalizeSystemState(initialState);
        var normalizedMaterial = norm.NormalizeMaterial(material);

        var state = new SolutionState(normalizedState, [normalizedMaterial], conditions);

        return state;
    }

    private static SolutionState Symmetric3PointBoundary(ISinteringConditions conditions)
    {
        var nodeCountPerParticle = 50;

        var particle1 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, Guid.NewGuid())
        {
            NodeCount = nodeCountPerParticle,
        }.GetParticle();

        var particle2 = new ShapeFunctionParticleFactory(100e-6, 0.2, 5, 0.2, particle1.MaterialId)
        {
            NodeCount = nodeCountPerParticle,
            RotationAngle = Angle.Half,
            CenterCoordinates = (300e-6, 0),
        }.GetParticle();

        var material = new Material(
            particle1.MaterialId,
            "Al2O3",
            new BulkProperties(0, 1e-4),
            new SubstanceProperties(1.8e3, 101.96e-3),
            new InterfaceProperties(1.65e-10, 0.9),
            new Dictionary<Guid, IInterfaceProperties>
            {
                { particle2.MaterialId, new InterfaceProperties(1.65e-10, 0.5) },
            }
        );

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2]);
        var norm = new DefaultNormalizer().GetNorm(initialState, conditions, [material]);
        var normalizedState = norm.NormalizeSystemState(initialState);
        var normalizedMaterial = norm.NormalizeMaterial(material);

        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 0.5e-2).Solve(
            normalizedState
        );

        var state = new SolutionState(compactedState, [normalizedMaterial], conditions);

        return state;
    }
}
