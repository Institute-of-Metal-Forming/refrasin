using RefraSin.MaterialData;

namespace RefraSin.ProcessModel.Sintering;

/// <summary>
/// Data structure representing a sintering process.
/// </summary>
public class SinteringStep : ProcessStepBase, ISinteringStep
{
    /// <summary>
    /// Creates a new sintering process.
    /// </summary>
    /// <param name="duration">duration of the process step</param>
    /// <param name="temperature">the process temperature</param>
    /// <param name="solver">the solver to use</param>
    /// <param name="gasConstant">value of the universal gas constant</param>
    /// <param name="materials"></param>
    /// <param name="materialInterfaces"></param>
    public SinteringStep(
        double duration,
        double temperature, IProcessStepSolver<SinteringStep> solver, IReadOnlyList<IMaterial> materials, IReadOnlyList<IMaterialInterface> materialInterfaces, double gasConstant = 8.31446261815324
    )
    {
        Duration = duration;
        Temperature = temperature;
        Solver = solver;
        Materials = materials;
        MaterialInterfaces = materialInterfaces;
        Temperature = temperature;
        GasConstant = gasConstant;
    }

    /// <summary>
    /// Duration of the sintering step.
    /// </summary>
    public double Duration { get; }

    /// <inheritdoc />
    public double Temperature { get; }

    /// <inheritdoc />
    public double GasConstant { get; }

    /// <inheritdoc />
    public IProcessStepSolver<SinteringStep> Solver { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterial> Materials { get; }

    /// <inheritdoc />
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }

    /// <inheritdoc />
    public override ISystemState Solve(ISystemState inputState) => Solver.Solve(this, inputState);
}