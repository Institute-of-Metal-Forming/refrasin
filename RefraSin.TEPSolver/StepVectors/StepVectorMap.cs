using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(SolutionState currentState)
    {
        _index = 0;

        foreach (var particle in currentState.Particles)
        {
            var startIndex = _index;

            foreach (var node in particle.Nodes)
            {
                AddUnknown(node.Id, Unknown.NormalDisplacement);
                if (node is NeckNode)
                    AddUnknown(node.Id, Unknown.TangentialDisplacement);
                AddUnknown(node.Id, Unknown.FluxToUpper);
                AddUnknown(node.Id, Unknown.LambdaVolume);
            }

            _particleBlocks[particle.Id] = (startIndex, _index - startIndex);
        }

        foreach (var contact in currentState.ParticleContacts)
        {
            var startIndex = _index;

            foreach (var contactNode in contact.FromNodes)
            {
                AddUnknown(contactNode.Id, Unknown.LambdaContactDistance);
                AddUnknown(contactNode.Id, Unknown.LambdaContactDirection);
                LinkUnknown(
                    contactNode.Id,
                    contactNode.ContactedNodeId,
                    Unknown.LambdaContactDistance
                );
                LinkUnknown(
                    contactNode.Id,
                    contactNode.ContactedNodeId,
                    Unknown.LambdaContactDirection
                );
            }

            AddUnknown(contact.MergedId, Unknown.RadialDisplacement);
            AddUnknown(contact.MergedId, Unknown.AngleDisplacement);
            AddUnknown(contact.MergedId, Unknown.RotationDisplacement);

            _contactBlocks[(contact.From.Id, contact.To.Id)] = (startIndex, _index - startIndex);
        }

        GlobalStart = _index;

        AddUnknown(Guid.Empty, Unknown.LambdaDissipation);

        GlobalLength = _index - GlobalStart;
        TotalLength = _index;
    }

    public int TotalLength { get; }

    private void AddUnknown(Guid id, Unknown unknown)
    {
        _indices[(id, unknown)] = _index;
        _index++;
    }

    private void LinkUnknown(Guid existingId, Guid newId, Unknown unknown)
    {
        _indices[(newId, unknown)] = _indices[(existingId, unknown)];
    }

    private int _index;
    private readonly Dictionary<(Guid, Unknown), int> _indices = new();
    private readonly Dictionary<Guid, (int start, int length)> _particleBlocks = new();
    private readonly Dictionary<(Guid, Guid), (int start, int length)> _contactBlocks = new();

    public (int start, int length) this[IParticle particle] => _particleBlocks[particle.Id];

    public (int start, int length) this[IParticleContactEdge contact] =>
        _contactBlocks[(contact.From, contact.To)];

    public int GlobalStart { get; }

    public int GlobalLength { get; }

    public int LambdaDissipation() => _indices[(Guid.Empty, Unknown.LambdaDissipation)];

    public int NormalDisplacement(INode node) => _indices[(node.Id, Unknown.NormalDisplacement)];

    public int FluxToUpper(INode node) => _indices[(node.Id, Unknown.FluxToUpper)];

    public int LambdaVolume(INode node) => _indices[(node.Id, Unknown.LambdaVolume)];

    public int TangentialDisplacement(INode node) =>
        _indices[(node.Id, Unknown.TangentialDisplacement)];

    public int LambdaContactDistance(INode node) =>
        _indices[(node.Id, Unknown.LambdaContactDistance)];

    public int LambdaContactDirection(INode node) =>
        _indices[(node.Id, Unknown.LambdaContactDirection)];

    public int RadialDisplacement(ParticleContact contact) =>
        _indices[(contact.MergedId, Unknown.RadialDisplacement)];

    public int AngleDisplacement(ParticleContact contact) =>
        _indices[(contact.MergedId, Unknown.AngleDisplacement)];

    public int RotationDisplacement(ParticleContact contact) =>
        _indices[(contact.MergedId, Unknown.RotationDisplacement)];

    private enum Unknown
    {
        NormalDisplacement,
        TangentialDisplacement,
        FluxToUpper,
        LambdaVolume,
        LambdaContactDistance,
        LambdaContactDirection,
        RadialDisplacement,
        AngleDisplacement,
        RotationDisplacement,
        LambdaDissipation,
    }
}
