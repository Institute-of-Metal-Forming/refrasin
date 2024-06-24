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
                AddNodeUnknown(node, NodeUnknown.LambdaVolume);
                AddNodeUnknown(node, NodeUnknown.FluxToUpper);
                AddNodeUnknown(node, NodeUnknown.NormalDisplacement);
            }

            AddParticleUnknown(particle, ParticleUnknown.LambdaDissipation);

            _particleBlocks[particle.Id] = (startIndex, _index - startIndex);
        }

        BorderStart = _index;

        foreach (var contact in currentState.Contacts)
        {
            AddContactUnknown(contact, ContactUnknown.RadialDisplacement);
            AddContactUnknown(contact, ContactUnknown.AngleDisplacement);
            AddContactUnknown(contact, ContactUnknown.RotationDisplacement);

            foreach (var contactNode in contact.FromNodes)
            {
                AddNodeUnknown(contactNode, NodeUnknown.LambdaContactDistance);
                AddNodeUnknown(contactNode, NodeUnknown.LambdaContactDirection);
                
                if (contactNode is ParticleModel.NeckNode)
                    AddNodeUnknown(contactNode, NodeUnknown.TangentialDisplacement);
            }

            foreach (var contactNode in contact.ToNodes)
            {
                LinkCommonNodeUnknown(contactNode, NodeUnknown.LambdaContactDistance);
                LinkCommonNodeUnknown(contactNode, NodeUnknown.LambdaContactDirection);
                
                if (contactNode is ParticleModel.NeckNode)
                    AddNodeUnknown(contactNode, NodeUnknown.TangentialDisplacement);
            }
        }

        BorderLength = _index - BorderStart;
    }

    private void AddNodeUnknown(INode node, NodeUnknown unknown)
    {
        _nodeUnknownIndices[(node.Id, unknown)] = _index;
        _index++;
    }

    private void AddParticleUnknown(IParticle particle, ParticleUnknown unknown)
    {
        _particleUnknownIndices[(particle.Id, unknown)] = _index;
        _index++;
    }

    private void AddContactUnknown(IParticleContact contact, ContactUnknown unknown)
    {
        _contactUnknownIndices[(contact.From.Id, contact.To.Id, unknown)] = _index;
        _index++;
    }

    private void LinkCommonNodeUnknown(IContactNode childsNode, NodeUnknown unknown)
    {
        _nodeUnknownIndices[(childsNode.Id, unknown)] = _nodeUnknownIndices[(childsNode.ContactedNodeId, unknown)];
    }

    private int _index;
    private readonly Dictionary<(Guid, NodeUnknown), int> _nodeUnknownIndices = new();
    private readonly Dictionary<(Guid, ParticleUnknown), int> _particleUnknownIndices = new();
    private readonly Dictionary<(Guid, Guid, ContactUnknown), int> _contactUnknownIndices = new();
    private readonly Dictionary<Guid, (int start, int length)> _particleBlocks = new();

    public int this[IParticle particle, ParticleUnknown unknown] => _particleUnknownIndices[(particle.Id, unknown)];

    public int this[Guid particleId, ParticleUnknown unknown] => _particleUnknownIndices[(particleId, unknown)];

    public int this[INode node, NodeUnknown unknown] => _nodeUnknownIndices[(node.Id, unknown)];

    public int this[Guid nodeId, NodeUnknown unknown] => _nodeUnknownIndices[(nodeId, unknown)];

    public int this[IParticleContact contact, ContactUnknown unknown] => _contactUnknownIndices[(contact.From.Id, contact.To.Id, unknown)];

    public int this[Guid fromId, Guid toId, ContactUnknown unknown] => _contactUnknownIndices[(fromId, toId, unknown)];

    public (int start, int length) this[IParticle particle] => _particleBlocks[particle.Id];

    public int BorderStart { get; }

    public int BorderLength { get; }
}