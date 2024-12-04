using System.Globalization;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Plotly.NET;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.Plotting;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
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

    public static IEnumerable<TestFixtureData> GetTestFixtureData() =>
        InitialStates
            .Generate(Conditions)
            .Select(s => new TestFixtureData(s.state) { TestName = s.label });

    private string _tmpDir = TempPath.CreateTempDir();
    private readonly SolutionState _state;
    private static readonly ISinteringConditions Conditions = new SinteringConditions(2073, 0);

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
}
