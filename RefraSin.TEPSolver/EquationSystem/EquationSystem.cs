using MathNet.Numerics.LinearAlgebra;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class EquationSystem
{
    internal EquationSystem(
        SolutionState solutionState,
        IEnumerable<IQuantity> quantities,
        IEnumerable<IConstraint> constraints
    )
    {
        State = solutionState;
        Constraints = constraints.ToArray();
        Quantities = quantities.ToArray();
    }

    public SolutionState State { get; }

    public IReadOnlyList<IQuantity> Quantities { get; }
    public IReadOnlyList<IConstraint> Constraints { get; }

    public double Dissipation(StepVector stepVector)
    {
        throw new NotImplementedException();
    }

    public Vector<double> Lagrangian() => throw new NotImplementedException();

    public Matrix<double> Jacobian() => throw new NotImplementedException();
}
