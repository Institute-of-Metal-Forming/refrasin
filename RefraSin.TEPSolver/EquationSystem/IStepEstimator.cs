using RefraSin.ProcessModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

public interface IStepEstimator
{
    StepVector EstimateStep(IProcessConditions conditions, SolutionState currentState);
}