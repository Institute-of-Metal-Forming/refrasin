using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Pores;

public class PoreState<TNode>(
    Guid id,
    IEnumerable<TNode> nodes,
    double porosity,
    double elasticStrain = 0
) : Pore<TNode>(id, nodes), IPoreState<TNode>
    where TNode : INode
{
    public double Porosity { get; } = porosity;

    public double ElasticStrain { get; } = elasticStrain;
}
