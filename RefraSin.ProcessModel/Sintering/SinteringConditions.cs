namespace RefraSin.ProcessModel.Sintering;

public record SinteringConditions(
    double Temperature,
    double Duration,
    double GasConstant = 8.31446261815324
) : ISinteringConditions { }
