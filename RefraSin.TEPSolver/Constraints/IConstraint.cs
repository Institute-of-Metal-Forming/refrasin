using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public interface IConstraint : ISystemItem
{
    double Residual(EquationSystem equationSystem, StepVector stepVector);

    IEnumerable<(int index, double value)> Derivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    );

    IEnumerable<(int firstIndex, int secondIndex, double value)> SecondDerivatives(
        EquationSystem equationSystem,
        StepVector stepVector
    ) => [];
}
