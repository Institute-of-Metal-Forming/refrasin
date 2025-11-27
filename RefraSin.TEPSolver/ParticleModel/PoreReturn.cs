using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.TEPSolver.Normalization;
using RefraSin.Vertex;

namespace RefraSin.TEPSolver.ParticleModel;

internal record PoreReturn : IPoreState<NodeReturn>
{
    public PoreReturn(
        Guid id,
        IEnumerable<NodeReturn> nodes,
        double relativeDensity,
        double pressure,
        INorm norm
    )
    {
        Id = id;
        Nodes = nodes.ToReadOnlyVertexCollection();
        RelativeDensity = relativeDensity;
        Pressure = pressure * norm.Mass / double.Pow(norm.Time, 2) / norm.Length;
        Volume = this.Volume<PoreReturn, NodeReturn>();
    }

    public Guid Id { get; }
    public double RelativeDensity { get; }
    public double Volume { get; }
    public double Pressure { get; }
    public IReadOnlyVertexCollection<NodeReturn> Nodes { get; }
}
