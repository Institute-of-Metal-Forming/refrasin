namespace RefraSin.ProcessModel;

public interface ISystemStateTransition
{
   Guid InputStateId { get; }
   
   Guid OutputStateId { get; }
}