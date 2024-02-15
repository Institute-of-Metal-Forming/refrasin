using RefraSin.ProcessModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public interface IStepEstimator
{
    StepVector EstimateStep(IProcessConditions conditions, SolutionState currentState);
}