using Parquet.Serialization.Attributes;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParquetStorage;

public class NodeData : INode
{
    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <inheritdoc />
    [ParquetIgnore]
    public Guid ParticleId { get; set; }

    public PolarPointData Coordinates { get; set; }

    /// <inheritdoc />
    IPolarPoint INode.Coordinates =>
        new Coordinates.Polar.PolarPoint(Coordinates.Phi, Coordinates.R);

    /// <inheritdoc />
    public NodeType Type { get; set; }

    public static NodeData From(INode node)
    {
        var self = new NodeData()
        {
            Id = node.Id,
            ParticleId = node.ParticleId,
            Type = node.Type,
            Coordinates = PolarPointData.From(node.Coordinates),
        };

        return self;
    }
}
