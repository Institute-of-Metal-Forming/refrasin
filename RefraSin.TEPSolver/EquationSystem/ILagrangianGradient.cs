using MathNet.Numerics.LinearAlgebra;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public interface ILagrangianGradient
{
    StepVector EvaluateAt(IProcessConditions conditions, SolutionState currentState, StepVector currentEstimation);

    Matrix<double> EvaluateJacobianAt(IProcessConditions conditions, SolutionState currentState, StepVector currentEstimation);
}