using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver.TimeIntegration.Validation;

public interface IStepValidator
{
    public void Validate(ISolverSession solverSession, StepVector stepVector);
}