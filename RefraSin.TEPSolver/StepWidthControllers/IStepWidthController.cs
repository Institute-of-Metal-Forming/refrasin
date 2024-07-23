using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepWidthControllers;

public interface IStepWidthController : ISolverRoutine
{
    double GetStepWidth(ISolverSession solverSession, SolutionState currentState, StepVector stepVector);
}