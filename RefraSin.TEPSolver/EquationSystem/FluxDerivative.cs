using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class FluxDerivative : NodeEquationBase<NodeBase>
{
    /// <inheritdoc />
    public FluxDerivative(NodeBase node, StepVector step)
        : base(node, step) { }

    /// <inheritdoc />
    public override double Value()
    {
        var dissipationTerm =
            2
            * Node.Particle.VacancyVolumeEnergy
            * Node.SurfaceDistance.ToUpper
            / Node.InterfaceDiffusionCoefficient.ToUpper
            * Step.FluxToUpper(Node)
            * Step.LambdaDissipation();
        var thisRequiredConstraintsTerm = Step.LambdaVolume(Node);
        var upperRequiredConstraintsTerm = Step.LambdaVolume(Node.Upper);

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative()
    {
        var bilinearPreFactor =
            -2
            * Node.Particle.VacancyVolumeEnergy
            * Node.SurfaceDistance.ToUpper
            / Node.InterfaceDiffusionCoefficient.ToUpper;

        yield return (Map.FluxToUpper(Node), bilinearPreFactor * Step.LambdaDissipation());
        yield return (Map.LambdaDissipation(), bilinearPreFactor * Step.FluxToUpper(Node));
        yield return (Map.LambdaVolume(Node), -1);
        yield return (Map.LambdaVolume(Node.Upper), 1);
    }
}
