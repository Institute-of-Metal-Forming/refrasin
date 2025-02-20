using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public class DissipationEqualityConstraint : IGlobalConstraint
{
    private DissipationEqualityConstraint(SolutionState state)
    {
        State = state;
    }

    public static IGlobalConstraint Create(SolutionState solutionState) =>
        new DissipationEqualityConstraint(solutionState);

    /// <inheritdoc />
    public double Residual(StepVector stepVector)
    {
        var dissipation = stepVector.StepVectorMap.Quantities.Sum(q =>
            q.DrivingForce(stepVector) * stepVector.QuantityValue(q)
        );

        var dissipationFunction = State
            .Nodes.Select(n => FluxFactor(n) * Pow(stepVector.QuantityValue<FluxToUpper>(n), 2))
            .Sum();

        return dissipation - dissipationFunction;
    }

    public IEnumerable<(int index, double value)> Derivatives(StepVector stepVector)
    {
        foreach (var quantity in stepVector.StepVectorMap.Quantities)
        {
            var drivingForce = quantity.DrivingForce(stepVector);
            if (drivingForce != 0)
                yield return (stepVector.StepVectorMap.QuantityIndex(quantity), drivingForce);
        }

        foreach (var n in State.Nodes)
        {
            yield return (
                stepVector.StepVectorMap.QuantityIndex<FluxToUpper>(n),
                -2 * FluxFactor(n) * stepVector.QuantityValue<FluxToUpper>(n)
            );
        }
    }

    public IEnumerable<(int firstIndex, int secondIndex, double value)> SecondDerivatives(
        StepVector stepVector
    )
    {
        foreach (var n in State.Nodes)
        {
            var index = stepVector.StepVectorMap.QuantityIndex<FluxToUpper>(n);
            yield return (index, index, -2 * FluxFactor(n));
        }
    }

    private static double FluxFactor(NodeBase n) =>
        n.Particle.VacancyVolumeEnergy
        * n.SurfaceDistance.ToUpper
        / n.InterfaceDiffusionCoefficient.ToUpper;

    public SolutionState State { get; }
}
