using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class MatrixPoreViscoElasticMaterialConstraint(Pore pore) : IPoreItem, IConstraint
{
    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var densityTerm = stepVector.ItemValue<PoreDensity>(Pore) / Pore.RelativeDensity;
        var viscoseTerm = Pore.Pressure / Pore.PoreMaterial.ViscoElastic.VolumeViscosity;
        var elasticTerm =
            stepVector.ItemValue<PorePressure>(Pore)
            / Pore.PoreMaterial.ViscoElastic.CompressionModulus;

        return densityTerm + viscoseTerm + elasticTerm;
    }

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        yield return (
            stepVector.StepVectorMap.ItemIndex<PoreDensity>(Pore),
            1 / Pore.RelativeDensity
        );
        yield return (
            stepVector.StepVectorMap.ItemIndex<PorePressure>(Pore),
            1 / Pore.PoreMaterial.ViscoElastic.CompressionModulus
        );
    }

    public Pore Pore { get; } = pore;

    public override string ToString() => $"material constraint for {Pore}";
}
