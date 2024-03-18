namespace RefraSin.ProcessModel;

public interface ISystemChange
{
   ISystemState InputState { get; }
   
   ISystemState OutputState { get; }
}