using RefraSin.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

public interface IStepValidator
{
    public void Validate(SolutionState currentState, StepVector stepVector, ISolverOptions options);
}