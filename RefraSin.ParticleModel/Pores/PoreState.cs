using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Pores;

public class PoreState<TNode>(
    Guid id,
    IEnumerable<TNode> nodes,
    double relativeDensity,
    double pressure = 0
) : Pore<TNode>(id, nodes), IPoreDensity, IPorePressure
    where TNode : INode
{
    public double RelativeDensity { get; } = relativeDensity;

    public double Pressure { get; } = pressure;
}
