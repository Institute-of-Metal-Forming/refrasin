using System.Globalization;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Plotly.NET;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepEstimators;
using ScottPlot;

namespace RefraSin.TEPSolver.Test;

[TestFixtureSource(nameof(GetTestFixtureData))]
public class EquationSystemTest(ISystemState<IParticle<IParticleNode>, IParticleNode> state)
{
    public static IEnumerable<TestFixtureData> GetTestFixtureData() =>
        InitialStates.Generate().Select(s => new TestFixtureData(s.state) { TestName = s.label });

    private string _tmpDir = TempPath.CreateTempDir();
    private static readonly ISinteringConditions Conditions = new SinteringConditions(2073, 0);
    private static readonly IMaterial Material = new Material(
        InitialStates.MaterialId,
        "Al2O3",
        new BulkProperties(0, 1e-4),
        new SubstanceProperties(1.8e3, 101.96e-3),
        new InterfaceProperties(1.65e-10, 0.9),
        new Dictionary<Guid, IInterfaceProperties>
        {
            { InitialStates.MaterialId, new InterfaceProperties(1.65e-10, 0.5) },
        }
    );

    [Test]
    public void TestEquationSystem()
    {
        var norm = new DefaultNormalizer().GetNorm(state, Conditions, [Material]);
        var normalizedState = norm.NormalizeSystemState(state);
        var normalizedMaterial = norm.NormalizeMaterial(Material);
        var normalizedConditions = norm.NormalizeConditions(Conditions);

        PlotState(normalizedState);

        var solutionState = new SolutionState(
            normalizedState,
            [normalizedMaterial],
            normalizedConditions
        );
        var guess = new StepEstimator().EstimateStep(normalizedConditions, solutionState);
        var equationSystem = new EquationSystem.EquationSystem(solutionState, guess);

        SaveEquationSystem(equationSystem);
    }

    private void SaveEquationSystem(EquationSystem.EquationSystem system)
    {
        var jac = system.Jacobian();
        jac.CoerceZero(1e-8);
        SaveMatrix(jac, -system.Lagrangian(), "full_jacobian");
        PlotJacobianStructure(jac, "full_jacobian");

        foreach (var p in state.Particles)
        {
            var pJac = system.ParticleBlockJacobian(p);
            pJac.CoerceZero(1e-8);
            SaveMatrix(pJac, -system.ParticleBlockLagrangian(p), $"particle_{p.Id}");
            PlotJacobianStructure(jac, $"particle_{p.Id}");
        }

        foreach (var c in state.ParticleContacts)
        {
            var contactJac = system.ContactBlockJacobian(c);
            contactJac.CoerceZero(1e-8);
            SaveMatrix(
                contactJac,
                -system.ContactBlockLagrangian(c),
                $"contact_{c.From.Id}_{c.To.Id}"
            );
            PlotJacobianStructure(jac, $"contact_{c.From.Id}_{c.To.Id}");
        }

        var globalJac = system.GlobalBlockJacobian();
        globalJac.CoerceZero(1e-8);
        SaveMatrix(globalJac, -system.GlobalBlockLagrangian(), "global");
        PlotJacobianStructure(jac, "global");
    }

    private void PlotState(ISystemState<IParticle<IParticleNode>, IParticleNode> state)
    {
        var plot = ParticlePlot.PlotParticles(state.Particles);
        plot.SaveHtml(Path.Combine(_tmpDir, "state.html"));
    }

    private void SaveMatrix(Matrix<double> matrix, Vector<double> rightSide, string name)
    {
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

        File.WriteAllText(Path.Combine(_tmpDir, $"{name}.txt"), builder.ToString(), Encoding.UTF8);
    }

    private void PlotJacobianStructure(Matrix<double> jacobian, string name)
    {
        var matrix = jacobian.PointwiseSign();

        var plt = new Plot();

        plt.Add.Heatmap(matrix.ToArray());
        plt.Layout.Frameless();
        plt.Axes.Margins(0, 0);

        plt.SavePng(Path.Combine(_tmpDir, $"{name}.png"), matrix.ColumnCount, matrix.RowCount);
    }
}
