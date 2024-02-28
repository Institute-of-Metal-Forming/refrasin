using RefraSin.ParticleModel;
using RefraSin.Storage;

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
                AddNodeUnknown(contactNode, NodeUnknown.TangentialDisplacement);
                AddNodeUnknown(contactNode, NodeUnknown.LambdaContactDistance);
                AddNodeUnknown(contactNode, NodeUnknown.LambdaContactDirection);
            }
            
            foreach (var contactNode in contact.ToNodes)
            {
                AddNodeUnknown(contactNode, NodeUnknown.TangentialDisplacement);
                LinkCommonNodeUnknown(contactNode, NodeUnknown.LambdaContactDistance);
                LinkCommonNodeUnknown(contactNode, NodeUnknown.LambdaContactDirection);
            }
        }

        BorderLength = _index - BorderStart + 1;
    }

    private void AddNodeUnknown(INode node, NodeUnknown unknown)
    {
        _nodeUnknownIndices[(node.Id, unknown)] = _index;
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
    private readonly Dictionary<(Guid, Guid, ContactUnknown), int> _contactUnknownIndices = new();
    private readonly Dictionary<Guid, (int start, int length)> _particleBlocks = new();

    public int this[GlobalUnknown unknown] => _index + (int)unknown;

    public int this[INode node, NodeUnknown unknown] => _nodeUnknownIndices[(node.Id, unknown)];

    public int this[Guid nodeId, NodeUnknown unknown] => _nodeUnknownIndices[(nodeId, unknown)];

    public int this[IParticleContact contact, ContactUnknown unknown] => _contactUnknownIndices[(contact.From.Id, contact.To.Id, unknown)];

    public int this[Guid fromId, Guid toId, ContactUnknown unknown] => _contactUnknownIndices[(fromId, toId, unknown)];

    public (int start, int length) this[IParticle particle] => _particleBlocks[particle.Id];

    public int BorderStart { get; }

    public int BorderLength { get; }
}