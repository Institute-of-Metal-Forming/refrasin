using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public interface IStateVelocity : IQuantity
{
    public double DrivingForce(StepVector stepVector);
}
