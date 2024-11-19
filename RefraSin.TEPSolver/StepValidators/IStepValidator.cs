using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

public interface IStepValidator : ISolverRoutine
{
    public void Validate(SolutionState currentState, StepVector stepVector);
}
