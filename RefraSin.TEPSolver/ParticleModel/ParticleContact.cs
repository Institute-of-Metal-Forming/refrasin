using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.TEPSolver.ParticleModel;

public record ParticleContact : ParticleContactEdge<Particle>
{
    private IList<ContactNodeBase>? _fromNodes;
    private IList<ContactNodeBase>? _toNodes;

    /// <inheritdoc />
    public ParticleContact(Particle from, Particle to)
        : base(from, to)
    {
        var bytes1 = from.Id.ToByteArray();
        var bytes2 = to.Id.ToByteArray();
        var xoredBytes = new byte[16];

        for (var i = 0; i < 16; i++)
        {
            xoredBytes[i] = (byte)(bytes1[i] ^ bytes2[i]);
        }

        MergedId = new Guid(xoredBytes);
    }

    public IList<ContactNodeBase> FromNodes =>
        _fromNodes ??= From
            .Nodes.OfType<ContactNodeBase>()
            .Where(n => n.ContactedParticleId == To.Id)
            .ToArray();

    public IList<ContactNodeBase> ToNodes =>
        _toNodes ??= FromNodes.Select(n => n.ContactedNode).ToArray();
    
    public Guid MergedId { get; }
}
