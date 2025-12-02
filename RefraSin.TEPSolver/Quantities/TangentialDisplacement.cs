using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class TangentialDisplacement(NodeBase node) : INodeItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector)
    {
        var interfaceEnergyTerm = -Node.GibbsEnergyGradient.Tangential;

        var porePressureTerm = 0.0;
        // if (Node.Particle.SolutionState.NodesOfPores.TryGetValue(Node, out var pore))
        //     porePressureTerm = pore.HydrostaticStress * Node.VolumeGradient.Tangential;

        return interfaceEnergyTerm + porePressureTerm;
    }

    public NodeBase Node { get; } = node;

    public override string ToString() => $"tangential displacement of {Node}";
}
