using static System.Double;
using static System.Math;

namespace RefraSin.Iteration;

/// <summary>
/// Encapsulates functionality for shooting iterations to determine the value of a <see cref="Variable"/> in that manner, that the result from a calculation is <see cref="Goal"/>.
/// Serves an optional fluent interface for changing properties after creation.
/// </summary>
public class ShootingIterator
{
    private double _precision;
    private double _precisionFactor;
    private double _precisionNorm = NaN;
    private double _maximumChange = PositiveInfinity;

    /// <summary>
    /// Creates an instance with the specified goal value, initial change and the desired precision.
    /// </summary>
    /// <param name="goal">goal value</param>
    /// <param name="initialValue">initial value of <see cref="Variable"/></param>
    /// <param name="initialChangeFactor">initial change as fraction of <paramref name="initialValue"/></param>
    /// <param name="precision">desired precision</param>
    public ShootingIterator(double goal, double initialValue, double initialChangeFactor, double precision)
    {
        Goal = goal;
        Variable = initialValue;
        Precision = precision;
        Change = initialChangeFactor * initialValue;
    }

    /// <summary>
    /// Creates an instance with the specified goal value and norm. Initial change and the desired precision are given as fraction of <paramref name="precisionNorm"/>.
    /// </summary>
    /// <param name="goal">goal value</param>
    /// <param name="initialValue">initial value of <see cref="Variable"/></param>
    /// <param name="initialChangeFactor">initial change as fraction of <paramref name="initialValue"/></param>
    /// <param name="precisionNorm">norm to scale <paramref name="precisionFactor"/></param>
    /// <param name="precisionFactor">desired precision value as fraction of <paramref name="precisionNorm"/></param>
    public ShootingIterator(double goal, double initialValue, double initialChangeFactor, double precisionNorm, double precisionFactor)
    {
        Goal = goal;
        Variable = initialValue;
        PrecisionNorm = precisionNorm;
        PrecisionFactor = precisionFactor;
        Change = initialChangeFactor * initialValue;
    }

    /// <summary>
    /// The actual result value of the previous iteration.
    /// </summary>
    public double? PreviousResult { get; private set; }

    /// <summary>
    /// The goal result value of the iteration.
    /// </summary>
    public double Goal { get; set; }

    /// <summary>
    /// The actual result value of the iteration.
    /// </summary>
    public double? CurrentResult { get; private set; }

    /// <summary>
    /// The change applied at <see cref="Variable"/>.
    /// </summary>
    public double Change { get; private set; }

    /// <summary>
    /// The current value of the iterational variable.
    /// </summary>
    public double Variable { get; private set; }

    /// <summary>
    /// Determines if <see cref="Goal"/> and <see cref="CurrentResult"/> are equal within numerical <see cref="Precision"/>.
    /// </summary>
    public bool IsSatisfied => Abs(Goal - CurrentResult ?? NaN) < Abs(Precision);

    /// <summary>
    /// The absolute desired precision.
    /// </summary>
    public double Precision
    {
        get => _precision;
        set
        {
            _precision = value;
            _precisionFactor = _precision / _precisionNorm;
        }
    }

    /// <summary>
    /// The desired precision relative to <see cref="PrecisionNorm"/>.
    /// </summary>
    public double PrecisionFactor
    {
        get => _precisionFactor;
        set
        {
            _precisionFactor = value;
            _precision = _precisionFactor * _precisionNorm;
        }
    }

    /// <summary>
    /// The norm of <see cref="PrecisionFactor"/>.
    /// </summary>
    public double PrecisionNorm
    {
        get => _precisionNorm;
        set
        {
            _precisionNorm = value;
            _precision = _precisionFactor * _precisionNorm;
        }
    }

    /// <summary>
    /// The maximal change value allowed. If exceeded, the change is decreased by division through 2 until it is small enough.
    /// </summary>
    public double MaximumChange
    {
        get => _maximumChange;
        set => _maximumChange = Abs(value);
    }

    /// <summary>
    /// Adjusts the <see cref="Change"/> based on the new <paramref name="result"/>.
    /// </summary>
    /// <param name="result">new result value</param>
    /// <returns>this instance</returns>
    public virtual ShootingIterator AdjustChange(double result)
    {
        PreviousResult = CurrentResult;
        CurrentResult = result;
            
        if(PreviousResult.HasValue)
            Change *= (result - Goal) / (PreviousResult.Value - result);

        while (Abs(Change) > MaximumChange) Change /= 2;

        return this;
    }

    /// <summary>
    /// Applies the <see cref="Change"/> to <see cref="Variable"/>.
    /// </summary>
    /// <returns>this instance</returns>
    public virtual ShootingIterator ApplyChange()
    {
        Variable += Change;
        return this;
    }

    /// <summary>
    /// Adjusts the <see cref="Change"/> and applies it to <see cref="Variable"/> based on the new <paramref name="result"/> value.
    /// Convenience for <c>this.AdjustChange(result).ApplyChange()</c>.
    /// </summary>
    /// <param name="result">new result value</param>
    /// <returns>this instance</returns>
    public ShootingIterator AdjustAndApplyChange(double result)
        => AdjustChange(result).ApplyChange();

    /// <summary>
    /// Sets <see cref="Goal"/> to <paramref name="goal"/>.
    /// </summary>
    /// <param name="goal"></param>
    /// <returns>this instance</returns>
    public ShootingIterator WithGoal(double goal)
    {
        Goal = goal;
        return this;
    }

    /// <summary>
    /// Sets <see cref="Precision"/> to <paramref name="precision"/>.
    /// </summary>
    /// <param name="precision"></param>
    /// <returns>this instance</returns>
    public ShootingIterator WithPrecision(double precision)
    {
        Precision = precision;
        return this;
    }

    /// <summary>
    /// Sets <see cref="PrecisionNorm"/> to <paramref name="norm"/>.
    /// </summary>
    /// <param name="norm"></param>
    /// <returns>this instance</returns>
    public ShootingIterator WithPrecisionNorm(double norm)
    {
        PrecisionNorm = norm;
        return this;
    }

    /// <summary>
    /// Sets <see cref="PrecisionFactor"/> to <paramref name="precisionFactor"/>.
    /// </summary>
    /// <param name="precisionFactor"></param>
    /// <returns>this instance</returns>
    public ShootingIterator WithPrecisionFactor(double precisionFactor)
    {
        PrecisionFactor = precisionFactor;
        return this;
    }
}