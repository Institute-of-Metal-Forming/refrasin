using System.Globalization;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.FSharp.Core;
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

public class EquationSystemTest
{
    private string _tmpDir = TempPath.CreateTempDir();

    private void PlotState(ISystemState<IParticle<IParticleNode>, IParticleNode> state)
    {
        var plot = ParticlePlot.PlotParticles(state.Particles);
        plot.SaveHtml(Path.Combine(_tmpDir, "state.html"));
    }

    private void SaveMatrix(Matrix<double> matrix, string name)
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

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    [Test]
    public void TestSymmetric3PointBoundary()
    {
        var nodeCountPerParticle = 50;

        var conditions = new SinteringConditions(1200, 0);

        var particle1 = new ShapeFunctionParticleFactory(100, 0.2, 5, 0.2, Guid.NewGuid())
        {
            NodeCount = nodeCountPerParticle,
        }.GetParticle();

        var particle2 = new ShapeFunctionParticleFactory(100, 0.2, 5, 0.2, particle1.MaterialId)
        {
            NodeCount = nodeCountPerParticle,
            RotationAngle = Angle.Half,
            CenterCoordinates = (300, 0)
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
        var norm = new DefaultNormalizer().GetNorm(initialState, conditions, [material]);
        var normalizedState = norm.NormalizeSystemState(initialState);
        var normalizedMaterial = norm.NormalizeMaterial(material);

        var compactedState = new FocalCompactionStep(new AbsolutePoint(0, 0), 0.5).Solve(
            normalizedState
        );

        var state = new SolutionState(compactedState, [normalizedMaterial], conditions);
        PlotState(state);

        var guess = new StepEstimator().EstimateStep(conditions, state);

        var jac = Jacobian.EvaluateAt(state, guess);
        jac.CoerceZero(1e-8);
        SaveMatrix(jac, "full_jacobian");

        foreach (var p in state.Particles)
        {
            var pJac = Jacobian.ParticleBlock(p, guess);
            pJac.CoerceZero(1e-8);
            SaveMatrix(pJac, $"particle_{p.Id}");
        }

        var linBorderJac = Jacobian.LinearBorderBlock(state, guess);
        linBorderJac.CoerceZero(1e-8);
        SaveMatrix(linBorderJac, "linear_border");

        var borderJac = Jacobian.BorderBlock(state, guess);
        borderJac.CoerceZero(1e-8);
        SaveMatrix(borderJac, "border");
    }
}
