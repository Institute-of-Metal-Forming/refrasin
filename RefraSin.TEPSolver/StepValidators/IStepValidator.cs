using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

public interface IStepValidator
{
    public void Validate(ISolverSession solverSession, StepVector stepVector);
}