using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.Constraints;

public class DissipationEqualityConstraint : IGlobalConstraint
{
    private DissipationEqualityConstraint() { }

    public static IGlobalConstraint Create(SolutionState solutionState) =>
        new DissipationEqualityConstraint();

    /// <inheritdoc />
    public double Residual(StepVector stepVector)
    {
        var dissipation = stepVector
            .StepVectorMap.Quantities.OfType<IStateVelocity>()
            .Sum(q => q.DrivingForce(stepVector) * stepVector.QuantityValue(q));

        var dissipationFunction = stepVector
            .StepVectorMap.Quantities.OfType<IFlux>()
            .Sum(q => q.DissipationFactor(stepVector) * Pow(stepVector.QuantityValue(q), 2));

        Log.Logger.ForContext<DissipationEqualityConstraint>()
            .Debug(
                "Dissipation {Dissipation}, Dissipation Function {DissipationFunction}, Error {Error} ({ErrorPercent:f2}%).",
                dissipation,
                dissipationFunction,
                dissipation - dissipationFunction,
                (dissipation - dissipationFunction) / dissipation * 100
            );
        return dissipation - dissipationFunction;
    }

    public IEnumerable<(int index, double value)> Derivatives(StepVector stepVector)
    {
        foreach (var stateVelocity in stepVector.StepVectorMap.Quantities.OfType<IStateVelocity>())
        {
            var drivingForce = stateVelocity.DrivingForce(stepVector);
            if (drivingForce != 0)
                yield return (stepVector.StepVectorMap.QuantityIndex(stateVelocity), drivingForce);
        }

        foreach (var flux in stepVector.StepVectorMap.Quantities.OfType<IFlux>())
        {
            yield return (
                stepVector.StepVectorMap.QuantityIndex(flux),
                -2 * flux.DissipationFactor(stepVector) * stepVector.QuantityValue(flux)
            );
        }
    }

    public IEnumerable<(int firstIndex, int secondIndex, double value)> SecondDerivatives(
        StepVector stepVector
    )
    {
        foreach (var flux in stepVector.StepVectorMap.Quantities.OfType<IFlux>())
        {
            var index = stepVector.StepVectorMap.QuantityIndex(flux);
            yield return (index, index, -2 * flux.DissipationFactor(stepVector));
        }
    }

    public override string ToString() => "global dissipation equality constraint";
}
