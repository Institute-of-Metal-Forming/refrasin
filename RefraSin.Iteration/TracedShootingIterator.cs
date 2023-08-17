namespace RefraSin.Iteration;

/// <summary>
/// Encapsulates functionality for shooting iterations to determine the value of a <see cref="ShootingIterator.Variable"/> in that manner, that the result from a calculation is <see cref="ShootingIterator.Goal"/>.
/// Serves an optional fluent interface for changing properties after creation.
/// Traces the iteration progress in difference to <see cref="ShootingIterator"/>.
/// </summary>
public class TracedShootingIterator : ShootingIterator
{
    private readonly List<double> _variableTrace = new();
    private readonly List<double> _changeTrace = new();
    private readonly List<double> _resultTrace = new();

    /// <inheritdoc />
    public TracedShootingIterator(double goal, double initialValue, double initialChangeFactor, double precision)
        : base(goal, initialValue, initialChangeFactor, precision)
    {
        _variableTrace.Add(Variable);
    }

    /// <inheritdoc />
    public TracedShootingIterator(double goal, double initialValue, double precisionNorm, double initialChangeFactor, double precisionFactor)
        : base(goal, initialValue, initialChangeFactor, precisionNorm, precisionFactor)
    {
        _variableTrace.Add(Variable);
    }

    /// <summary>
    /// Sequence of the values of <see cref="ShootingIterator.Variable"/>.
    /// </summary>
    public IReadOnlyList<double> VariableTrace => _variableTrace;

    /// <summary>
    /// Sequence of the values of <see cref="ShootingIterator.Change"/>.
    /// </summary>
    public IReadOnlyList<double> ChangeTrace => _changeTrace;

    /// <summary>
    /// Sequence of the result values given to <see cref="ShootingIterator.AdjustChange"/> or <see cref="ShootingIterator.AdjustAndApplyChange"/>.
    /// </summary>
    public IReadOnlyList<double> ResultTrace => _resultTrace;

    /// <inheritdoc />
    public override ShootingIterator ApplyChange()
    {
        base.ApplyChange();
        _variableTrace.Add(Variable);
        return this;
    }

    /// <inheritdoc />
    public override ShootingIterator AdjustChange(double result)
    {
        base.AdjustChange(result);
        _changeTrace.Add(Change);
        _resultTrace.Add(result);
        return this;
    }
}