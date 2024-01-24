namespace RefraSin.ProcessModel;

public interface IProcessConditions
{
    /// <summary>
    /// Constant process temperature.
    /// </summary>
    public double Temperature { get; }

    /// <summary>
    /// Universal gas constant R.
    /// </summary>
    public double GasConstant { get; }
}