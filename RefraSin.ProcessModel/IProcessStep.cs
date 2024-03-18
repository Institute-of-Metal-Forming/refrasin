namespace RefraSin.ProcessModel;

/// <summary>
/// Interface for classes representing a process step transforming one <see cref="ISystemState"/> to another.
/// </summary>
public interface IProcessStep
{
   /// <summary>
   /// Run the solution procedure of this step.
   /// </summary>
   /// <param name="inputState">the incoming state</param>
   /// <returns></returns>
   ISystemState Solve(ISystemState inputState);
}