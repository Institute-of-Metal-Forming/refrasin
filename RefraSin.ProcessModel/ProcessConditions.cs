namespace RefraSin.ProcessModel;

public record ProcessConditions(
    double Temperature,
    double GasConstant = 8.31446261815324
) : IProcessConditions { }