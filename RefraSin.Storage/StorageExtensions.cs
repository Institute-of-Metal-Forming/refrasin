using RefraSin.ProcessModel;

namespace RefraSin.Storage;

public static class StorageExtensions
{
    public static void UseStorage(this IProcessStep processStep, ISolutionStorage storage)
    {
        processStep.SystemStateReported += storage.HandleReportSystemState;
    }

    private static void HandleReportSystemState(this ISolutionStorage storage, object? sender, IProcessStep.SystemStateReportedEventArgs reportedEventArgs)
    {
        if (sender is not IProcessStep processStep)
            throw new InvalidOperationException($"Sender must be a {nameof(IProcessStep)}");
        
        storage.StoreState(processStep, reportedEventArgs.State);
    }
}