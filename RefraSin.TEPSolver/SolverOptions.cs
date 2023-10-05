using RefraSin.Coordinates;

namespace RefraSin.TEPSolver;

/// <summary>
/// Class holding options to customize solver behavior.
/// </summary>
public class SolverOptions : ISolverOptions
{
    /// <inheritdoc />
    public int MaxIterationCount { get; set; } = 100;

    /// <inheritdoc />
    public int RootFindingMaxIterationCount { get; set; } = 1000;

    /// <inheritdoc />
    public double RootFindingAccuracy { get; } = 1e-4;

    /// <inheritdoc />
    public double IterationPrecision { get; set; } = 0.01;

    /// <inheritdoc />
    public double DiscretizationWidth { get; set; } = 10e-3;

    /// <inheritdoc />
    public uint TimeStepIncreaseDelay { get; set; } = 3;

    /// <inheritdoc />
    public double MinTimeStepWidth { get; set; } = 1e-8;

    /// <inheritdoc />
    public double MaxTimeStepWidth { get; set; } = Double.PositiveInfinity;

    /// <inheritdoc />
    public double TimeStepAdaptationFactor { get; set; } = 2.0;

    /// <inheritdoc />
    public Angle MaxSurfaceDisplacementAngle { get; set; } = Angle.FromDegrees(5);

    /// <inheritdoc />
    public bool AddAdditionalGrainBoundaryNodes { get; set; } = true;

    /// <inheritdoc />
    public double RemeshingDistanceDeletionLimit { get; set; } = 0.5;

    /// <inheritdoc />
    public double RemeshingDistanceInsertionLimit { get; set; } = 1.6;

    /// <inheritdoc />
    public double GrainBoundaryRemeshingInsertionRatio { get; set; } = 1.0;

    /// <inheritdoc />
    public double InitialTimeStepWidth { get; set; } = 1;

    /// <inheritdoc />
    public double TimeOut { get; set; } = -1;
}