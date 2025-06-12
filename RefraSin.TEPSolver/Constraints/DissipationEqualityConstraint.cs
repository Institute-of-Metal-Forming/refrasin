using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.Constraints;

public class DissipationEqualityConstraint(SolutionState solutionState) : IGlobalItem, IConstraint
{
    /// <inheritdoc />
    public SolutionState SolutionState { get; } = solutionState;

    /// <inheritdoc />
    public double Residual(EquationSystem equationSystem, StepVector stepVector)
    {
        var dissipation = equationSystem
            .Items.OfType<IStateVelocity>()
            .Sum(q => q.DrivingForce(stepVector) * stepVector.ItemValue(q));

        var dissipationFunction = equationSystem
            .Items.OfType<IFlux>()
            .Sum(q => q.DissipationFactor(stepVector) * Pow(stepVector.ItemValue(q), 2));

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

    public IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        foreach (var stateVelocity in equationSystem.Items.OfType<IStateVelocity>())
        {
            var drivingForce = stateVelocity.DrivingForce(stepVector);
            if (drivingForce != 0)
                yield return (stepVector.StepVectorMap.ItemIndex(stateVelocity), drivingForce);
        }

        foreach (var flux in equationSystem.Items.OfType<IFlux>())
        {
            yield return (
                stepVector.StepVectorMap.ItemIndex(flux),
                -2 * flux.DissipationFactor(stepVector) * stepVector.ItemValue(flux)
            );
        }
    }

    public IEnumerable<(int firstIndex, int secondIndex, double value)> SecondDerivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    )
    {
        foreach (var flux in equationSystem.Items.OfType<IFlux>())
        {
            var index = stepVector.StepVectorMap.ItemIndex(flux);
            yield return (index, index, -2 * flux.DissipationFactor(stepVector));
        }
    }

    public override string ToString() => "global dissipation equality constraint";
}
