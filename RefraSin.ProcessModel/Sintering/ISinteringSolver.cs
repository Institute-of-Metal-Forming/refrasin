namespace RefraSin.ProcessModel.Sintering;

public interface ISinteringSolver
{
   ISystemState Solve(ISystemState inputState, ISinteringConditions conditions);
}