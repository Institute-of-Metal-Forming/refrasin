using RefraSin.ProcessModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepEstimators;

public interface IStepEstimator
{
    StepVector EstimateStep(IProcessConditions conditions, SolutionState currentState);
}