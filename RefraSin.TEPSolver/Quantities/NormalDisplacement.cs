using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class NormalDisplacement(NodeBase node) : IStateVelocity, INodeItem
{
    public double DrivingForce(StepVector stepVector)
    {
        var interfaceEnergyTerm = -Node.GibbsEnergyGradient.Normal;

        var porePressureTerm = 0.0;
        // if (Node.Particle.SolutionState.NodesOfPores.TryGetValue(Node, out var pore))
        //     porePressureTerm = pore.HydrostaticStress * Node.VolumeGradient.Normal;

        return interfaceEnergyTerm + porePressureTerm;
    }

    public NodeBase Node { get; } = node;

    public override string ToString() => $"normal displacement of {Node}";
}
