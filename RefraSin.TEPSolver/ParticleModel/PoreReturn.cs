using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.TEPSolver.Normalization;
using RefraSin.Vertex;

namespace RefraSin.TEPSolver.ParticleModel;

internal record PoreReturn : IPoreState<NodeReturn>, IPoreDenseVolume
{
    public PoreReturn(
        Guid id,
        IEnumerable<NodeReturn> nodes,
        double porosity,
        double elasticStrain,
        double denseVolume,
        INorm norm
    )
    {
        Id = id;
        Nodes = nodes.ToReadOnlyVertexCollection();
        Porosity = porosity;
        ElasticStrain = elasticStrain;
        DenseVolume = denseVolume * norm.Area;
        Volume = this.Volume<PoreReturn, NodeReturn>();
    }

    public Guid Id { get; }
    public double Porosity { get; }
    public double Volume { get; }
    public double DenseVolume { get; }
    public double ElasticStrain { get; }
    public IReadOnlyVertexCollection<NodeReturn> Nodes { get; }
}
