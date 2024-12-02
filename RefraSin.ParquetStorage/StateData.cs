using RefraSin.ProcessModel;

namespace RefraSin.ParquetStorage;

public class StateData : ISystemState
{
    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <inheritdoc />
    public double Time { get; set; }

    public static StateData From(ISystemState state) => new() { Id = state.Id, Time = state.Time };
}
