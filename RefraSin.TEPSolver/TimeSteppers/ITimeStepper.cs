using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public interface ITimeStepper :ISolverRoutine
{
    public StepVector Step(ISolverSession solverSession, SolutionState baseState, StepVector initialGuess);
}