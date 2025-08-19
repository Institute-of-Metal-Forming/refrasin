using RefraSin.TEPSolver.ParticleModel;
using Serilog;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.BreakConditions;

public class SolutionStuckBreakCondition(double breakRatio = 1e-4) : IBreakCondition
{
    public double BreakRatio { get; } = breakRatio;

    public void RegisterWithSolver(SinteringSolver solver)
    {
        solver.StepSuccessfullyCalculated += SolverOnStepSuccessfullyCalculated;
    }

    private void SolverOnStepSuccessfullyCalculated(
        object? sender,
        SinteringSolver.StepSuccessfullyCalculatedEventArgs e
    )
    {
        if (e.TimeStepWidth > _maxStep)
            _maxStep = e.TimeStepWidth;
        _lastStep = e.TimeStepWidth;
    }

    private double _maxStep = 0;
    private double _lastStep = 0;
    ILogger _logger => Log.ForContext<SolutionStuckBreakCondition>();

    public bool IsMet(SolutionState solutionState)
    {
        var criticalValue = _maxStep * BreakRatio;
        var result = _lastStep < criticalValue;

        if (result)
            _logger.Information(
                "Solution stuck condition met with time step width {Current} < {Critical}",
                _lastStep,
                criticalValue
            );
        else
            _logger.Debug(
                "Solution stuck condition not met with time step width {Current} > {Critical} ",
                _lastStep,
                criticalValue
            );

        return result;
    }
}
