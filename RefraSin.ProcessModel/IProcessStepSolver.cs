namespace RefraSin.ProcessModel;

public interface IProcessStepSolver<in TProcessStep> where TProcessStep : IProcessStep
{
    ISystemState Solve(TProcessStep processStep, ISystemState inputState);
}