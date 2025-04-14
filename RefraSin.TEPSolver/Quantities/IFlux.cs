using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public interface IFlux : IQuantity
{
    public double DissipationFactor(StepVector stepVector);
}
