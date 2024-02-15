using MathNet.Numerics.LinearAlgebra;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public interface ILagrangianGradient
{
    Vector<double> EvaluateAt(IProcessConditions conditions, SolutionState currentState, StepVector currentEstimation);
}