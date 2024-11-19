using System.Globalization;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public record Node(Guid Id, Guid ParticleId, IPolarPoint Coordinates, NodeType Type) : INode
{
    public Node(INode template)
        : this(template.Id, template.ParticleId, template.Coordinates, template.Type) { }

    public override string ToString() =>
        $"""{nameof(Node)}({Type}) @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}""";
}
