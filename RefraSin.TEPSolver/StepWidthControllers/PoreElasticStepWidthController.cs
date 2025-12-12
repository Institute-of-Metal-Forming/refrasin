using RefraSin.Coordinates.Helpers;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.StepWidthControllers;

public class PoreElasticStepWidthController(
    double initialTimeStepWidth = 1,
    double maximumRelativeStress = 0.2,
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
    public double? GetStepWidth(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        if (currentState.Pores.Count == 0)
            return null;

        var stepWidth = _stepWidths[solverSession.Id] * IncreaseFactor;

        while (true)
        {
            var stepWidth1 = stepWidth;
            var maxStress = currentState.Pores.Max(p =>
            {
                var internalSinteringForce =
                    -3
                    * (2 * p.PoreMaterial.SurfaceEnergy - p.PoreMaterial.GrainBoundaryEnergy)
                    / p.PoreMaterial.AverageParticleRadius
                    * Pow(1 - p.Porosity, 2)
                    * (2 * (1 - p.Porosity) - (1 - p.PoreMaterial.InitialPorosity))
                    / (1 - p.PoreMaterial.InitialPorosity);
                return Abs(
                    stepVector.ItemValue<PoreElasticStrain>(p)
                        * p.PorousCompressionModulus
                        * stepWidth1
                        / internalSinteringForce
                );
            });

            if (maxStress < MaximumRelativeStress)
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
    public double MaximumRelativeStress { get; } = maximumRelativeStress;
    public double IncreaseFactor { get; } = increaseFactor;
    public double DecreaseFactor { get; } = decreaseFactor;
    public double MinimalTimeStepWidth { get; } = minimalTimeStepWidth;
    public double MaximalTimeStepWidth { get; } = maximalTimeStepWidth;
}
