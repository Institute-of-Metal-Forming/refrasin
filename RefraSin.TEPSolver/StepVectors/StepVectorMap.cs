using RefraSin.ParticleModel;

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
                AddUnknown(node.Id, Unknown.LambdaVolume);
                AddUnknown(node.Id, Unknown.FluxToUpper);
                
                if (node is INeckNode)
                    AddUnknown(node.Id, Unknown.TangentialDisplacement);
                else
                    AddUnknown(node.Id, Unknown.NormalDisplacement);
            }

            _particleBlocks[particle.Id] = (startIndex, _index - startIndex);
        }

        BorderStart = _index;

        foreach (var contact in currentState.Contacts)
        {
            AddUnknown(contact.Id, Unknown.RadialDisplacement);
            AddUnknown(contact.Id, Unknown.AngleDisplacement);
            AddUnknown(contact.Id, Unknown.RotationDisplacement);

            foreach (var contactNode in contact.FromNodes)
            {
                AddUnknown(contactNode.Id, Unknown.LambdaContactDistance);
                AddUnknown(contactNode.Id, Unknown.LambdaContactDirection);

                if (contactNode is ParticleModel.NeckNode)
                    AddUnknown(contactNode.Id, Unknown.NormalDisplacement);
            }

            foreach (var contactNode in contact.ToNodes)
            {
                LinkUnknown(contactNode.ContactedNodeId, contactNode.Id, Unknown.LambdaContactDistance);
                LinkUnknown(contactNode.ContactedNodeId, contactNode.Id, Unknown.LambdaContactDirection);

                if (contactNode is ParticleModel.NeckNode)
                    AddUnknown(contactNode.Id, Unknown.NormalDisplacement);
            }
        }

        AddUnknown(Guid.Empty, Unknown.LambdaDissipation);

        BorderLength = _index - BorderStart;
    }

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

    public (int start, int length) this[IParticle particle] => _particleBlocks[particle.Id];

    public int BorderStart { get; }

    public int BorderLength { get; }
    public int LambdaDissipation() => _indices[(Guid.Empty, Unknown.LambdaDissipation)];

    public int NormalDisplacement(INode node) => _indices[(node.Id, Unknown.NormalDisplacement)];

    public int FluxToUpper(INode node) => _indices[(node.Id, Unknown.FluxToUpper)];

    public int LambdaVolume(INode node) => _indices[(node.Id, Unknown.LambdaVolume)];

    public int TangentialDisplacement(IContactNode node) => _indices[(node.Id, Unknown.TangentialDisplacement)];

    public int LambdaContactDistance(IContactNode node) => _indices[(node.Id, Unknown.LambdaContactDistance)];

    public int LambdaContactDirection(IContactNode node) => _indices[(node.Id, Unknown.LambdaContactDirection)];

    public int RadialDisplacement(IParticleContact contact) => _indices[(contact.Id, Unknown.RadialDisplacement)];

    public int AngleDisplacement(IParticleContact contact) => _indices[(contact.Id, Unknown.AngleDisplacement)];

    public int RotationDisplacement(IParticleContact contact) => _indices[(contact.Id, Unknown.RotationDisplacement)];

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