namespace RefraSin.ProcessModel.Sintering;

public interface ISinteringConditions
{
    /// <summary>
    /// Duration of the sintering process.
    /// </summary>
    public double Duration { get; }
    
    /// <summary>
    /// Constant process temperature.
    /// </summary>
    public double Temperature { get; }

    /// <summary>
    /// Universal gas constant R.
    /// </summary>
    public double GasConstant { get; }
}