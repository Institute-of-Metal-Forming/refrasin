using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepEstimators;

public interface IStepEstimator : ISolverRoutine
{
    StepVector EstimateStep(ISolverSession? solverSession, EquationSystem estimateSystem);
}
