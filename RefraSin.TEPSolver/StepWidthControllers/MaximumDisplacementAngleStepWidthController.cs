using RefraSin.Coordinates.Helpers;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.StepWidthControllers;

public class MaximumDisplacementAngleStepWidthController(
    double initialTimeStepWidth = 1,
    double maximumDisplacementAngle = 0.005,
    double increaseFactor = 2,
    double decreaseFactor = 0.8,
    double minimalTimeStepWidth = double.NegativeInfinity,
    double maximalTimeStepWidth = double.PositiveInfinity
) : IStepWidthController
{
    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver)
    {
        solver.SessionInitialized += SolverOnSessionInitialized;
        solver.StepRejected += SolverOnStepRejected;
    }

    private void SolverOnSessionInitialized(
        object? sender,
        SinteringSolver.SessionInitializedEventArgs e
    )
    {
        _stepWidths[e.SolverSession.Id] = InitialTimeStepWidth;
    }

    private void SolverOnStepRejected(object? sender, SinteringSolver.StepRejectedEventArgs e)
    {
        var logger = Log.ForContext<TrialAndErrorStepWidthController>();
        var currentStepWidth = _stepWidths[e.SolverSession.Id];

        if (currentStepWidth < MinimalTimeStepWidth)
            logger.Warning(
                "Time step width can not be decreased further, since it fall below the allowed minimum."
            );
        else
        {
            _stepWidths[e.SolverSession.Id] *= DecreaseFactor / IncreaseFactor;
            logger.Information(
                "Time step width decreased to {Step} due to invalid step.",
                _stepWidths[e.SolverSession.Id]
            );
        }
    }

    /// <inheritdoc />
    public double GetStepWidth(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var stepWidth = _stepWidths[solverSession.Id] * IncreaseFactor;

        while (true)
        {
            var stepWidth1 = stepWidth;
            var normalDisplacementAngles = currentState.Nodes.Select(n =>
            {
                var displacement = stepVector.QuantityValue<NormalDisplacement>(n) * stepWidth1;
                var upperAngle = SinLaw.Alpha(
                    displacement,
                    CosLaw.C(displacement, n.SurfaceDistance.ToUpper, n.SurfaceNormalAngle.ToUpper),
                    n.SurfaceNormalAngle.ToUpper
                );
                var lowerAngle = SinLaw.Alpha(
                    displacement,
                    CosLaw.C(displacement, n.SurfaceDistance.ToLower, n.SurfaceNormalAngle.ToLower),
                    n.SurfaceNormalAngle.ToLower
                );
                return double.Max(double.Abs(upperAngle), double.Abs(lowerAngle));
            });
            var maxNormalDisplacementAngle = normalDisplacementAngles.Max();

            var tangentialDisplacementAngles = currentState
                .Nodes.OfType<NeckNode>()
                .Select(n =>
                {
                    var displacement =
                        stepVector.QuantityValue<TangentialDisplacement>(n) * stepWidth1;
                    var upperAngle = SinLaw.Alpha(
                        displacement,
                        CosLaw.C(
                            displacement,
                            n.SurfaceDistance.ToUpper,
                            n.SurfaceTangentAngle.ToUpper
                        ),
                        n.SurfaceTangentAngle.ToUpper
                    );
                    var lowerAngle = SinLaw.Alpha(
                        displacement,
                        CosLaw.C(
                            displacement,
                            n.SurfaceDistance.ToLower,
                            n.SurfaceTangentAngle.ToLower
                        ),
                        n.SurfaceTangentAngle.ToLower
                    );
                    return double.Max(double.Abs(upperAngle), double.Abs(lowerAngle));
                });
            var maxTangentialDisplacementAngle = tangentialDisplacementAngles.Prepend(0).Max(); // prepend to avoid InvalidOperationException when no necks present

            var maxAngle = double.Max(maxNormalDisplacementAngle, maxTangentialDisplacementAngle);

            if (maxAngle < MaximumDisplacementAngle)
            {
                _stepWidths[solverSession.Id] = stepWidth;

                if (stepWidth < MinimalTimeStepWidth)
                    return MinimalTimeStepWidth;
                if (stepWidth > MaximalTimeStepWidth)
                    return MaximalTimeStepWidth;

                return stepWidth;
            }

            stepWidth *= DecreaseFactor;
        }
    }

    private readonly Dictionary<Guid, double> _stepWidths = new();

    public double InitialTimeStepWidth { get; } = initialTimeStepWidth;
    public double MaximumDisplacementAngle { get; } = maximumDisplacementAngle;
    public double IncreaseFactor { get; } = increaseFactor;
    public double DecreaseFactor { get; } = decreaseFactor;
    public double MinimalTimeStepWidth { get; } = minimalTimeStepWidth;
    public double MaximalTimeStepWidth { get; } = maximalTimeStepWidth;
}
