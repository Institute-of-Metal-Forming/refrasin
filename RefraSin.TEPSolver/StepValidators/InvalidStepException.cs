using RefraSin.Numerics.Exceptions;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

public class InvalidStepException : NumericException
{
    /// <inheritdoc />
    public InvalidStepException(
        SolutionState baseState,
        StepVector stepVector,
        string message = "Calculated time step was invalid."
    )
        : base(message)
    {
        BaseState = baseState;
        StepVector = stepVector;
    }

    public SolutionState BaseState { get; }
    public StepVector StepVector { get; }
}
