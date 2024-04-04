using RefraSin.ProcessModel;

namespace RefraSin.Storage;

public static class StorageExtensions
{
    public static void UseStorage(this IProcessStep processStep, ISolutionStorage storage)
    {
        processStep.SystemStateReported += storage.HandleReportSystemState;
        processStep.SystemStateTransitionReported += storage.HandleReportSystemChange;
    }

    private static void HandleReportSystemState(this ISolutionStorage storage, object? sender, IProcessStep.SystemStateReportedEventArgs reportedEventArgs)
    {
        if (sender is not IProcessStep processStep)
            throw new InvalidOperationException($"Sender must be a {nameof(IProcessStep)}");
        
        storage.StoreState(processStep, reportedEventArgs.State);
    }
    private static void HandleReportSystemChange(this ISolutionStorage storage, object? sender, IProcessStep.SystemStateTransitionReportedEventArgs reportedEventArgs)
    {
        if (sender is not IProcessStep processStep)
            throw new InvalidOperationException($"Sender must be a {nameof(IProcessStep)}");
        
        storage.StoreStateTransition(processStep, reportedEventArgs.StateTransition);
    }
}