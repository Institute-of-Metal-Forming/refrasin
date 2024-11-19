using System.Globalization;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.FSharp.Core;
using NUnit.Framework.Internal;
using Plotly.NET;
using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepEstimators;

namespace RefraSin.TEPSolver.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class EquationSystemTest
{
    public EquationSystemTest(SolutionState state)
    {
        _state = state;
    }

    public static IEnumerable<TestFixtureData> GetTestFixtureData()
    {
        TestFixtureData CreateTestFixtureData(string testName, Func<SolutionState> f) =>
            new(f()) { TestName = testName };

        yield return CreateTestFixtureData(nameof(OneParticle), OneParticle);
        yield return CreateTestFixtureData(
            nameof(Symmetric3PointBoundary),
            Symmetric3PointBoundary
        );
    }

    private string _tmpDir = TempPath.CreateTempDir();
    private readonly SolutionState _state;

    [Test]
    public void Test()
    {
        PlotState(_state);

        var guess = new StepEstimator().EstimateStep(Conditions, _state);
        var equationSystem = new EquationSystem.EquationSystem(_state, guess);

        SaveEquationSystem(equationSystem);
    }

    private void SaveEquationSystem(EquationSystem.EquationSystem system)
    {
        var jac = system.Jacobian();
        jac.CoerceZero(1e-8);
        SaveMatrix(jac, -system.Lagrangian(), "full_jacobian");

        foreach (var p in _state.Particles)
        {
            var pJac = system.ParticleBlockJacobian(p);
            pJac.CoerceZero(1e-8);
            SaveMatrix(pJac, -system.ParticleBlockLagrangian(p), $"particle_{p.Id}");
        }

        foreach (var c in _state.ParticleContacts)
        {
            var contactJac = system.ContactBlockJacobian(c);
            contactJac.CoerceZero(1e-8);
            SaveMatrix(
                contactJac,
                -system.ContactBlockLagrangian(c),
                $"contact_{c.From.Id}_{c.To.Id}"
            );
        }

        var globalJac = system.GlobalBlockJacobian();
        globalJac.CoerceZero(1e-8);
        SaveMatrix(globalJac, -system.GlobalBlockLagrangian(), "global");
    }

    private void PlotState(ISystemState<IParticle<IParticleNode>, IParticleNode> state)
    {
        var plot = ParticlePlot.PlotParticles(state.Particles);
        plot.SaveHtml(Path.Combine(_tmpDir, "state.html"));
    }

    private void SaveMatrix(Matrix<double> matrix, Vector<double> rightSide, string name)
    {
        var path = Path.Combine(_tmpDir, $"{name}.txt");

        var builder = new StringBuilder();
        builder.AppendLine(
            matrix.ToMatrixString(
                matrix.RowCount,
                matrix.ColumnCount,
                null,
                CultureInfo.InvariantCulture
            )
        );
        builder.AppendLine(
            $"Determinant: {matrix.Determinant().ToString(CultureInfo.InvariantCulture)}"
        );
        builder.AppendLine(
            $"Condition: {matrix.ConditionNumber().ToString(CultureInfo.InvariantCulture)}"
        );
        builder.AppendLine(
            $"IsSymmetric: {matrix.IsSymmetric().ToString(CultureInfo.InvariantCulture)}"
        );
        builder.AppendLine(
            "Secondary Determinants: "
                + string.Join(
                    ' ',
                    Enumerable
                        .Range(0, matrix.ColumnCount)
                        .Select(i =>
                        {
                            var m = matrix.Clone();
                            m.SetColumn(i, rightSide);
                            return m.Determinant().Round(8).ToString(CultureInfo.InvariantCulture);
                        })
                )
        );
        builder.AppendLine($"Rank: {matrix.Rank()}({matrix.Rank() - matrix.RowCount})");

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    private static readonly ISinteringConditions Conditions = new SinteringConditions(2073, 0);

    private static SolutionState OneParticle()
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
        var norm = new DefaultNormalizer().GetNorm(initialState, Conditions, [material]);
        var normalizedState = norm.NormalizeSystemState(initialState);
        var normalizedMaterial = norm.NormalizeMaterial(material);

        var state = new SolutionState(normalizedState, [normalizedMaterial], Conditions);

        return state;
    }

    private static SolutionState Symmetric3PointBoundary()
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
            CenterCoordinates = (300e-6, 0)
        }.GetParticle();

        var material = new Material(
            particle1.MaterialId,
            "Al2O3",
            new BulkProperties(0, 1e-4),
            new SubstanceProperties(1.8e3, 101.96e-3),
            new InterfaceProperties(1.65e-10, 0.9),
            new Dictionary<Guid, IInterfaceProperties>
            {
                { particle2.MaterialId, new InterfaceProperties(1.65e-10, 0.5) }
            }
        );

        var initialState = new SystemState(Guid.Empty, 0, [particle1, particle2]);
        var norm = new DefaultNormalizer().GetNorm(initialState, Conditions, [material]);
        var normalizedState = norm.NormalizeSystemState(initialState);
        var normalizedMaterial = norm.NormalizeMaterial(material);

        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 0.5e-6).Solve(
            normalizedState
        );

        var state = new SolutionState(compactedState, [normalizedMaterial], Conditions);

        return state;
    }
}
