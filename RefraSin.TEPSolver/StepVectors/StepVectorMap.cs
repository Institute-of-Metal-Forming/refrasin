using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(IEnumerable<IParticleContact> contacts, IEnumerable<INode> nodes)
    {
        _index = Enum.GetNames(typeof(GlobalUnknown)).Length;

        foreach (var node in nodes)
        {
            AddNodeUnknown(node, NodeUnknown.NormalDisplacement);
            AddNodeUnknown(node, NodeUnknown.FluxToUpper);
            AddNodeUnknown(node, NodeUnknown.LambdaVolume);

            if (node is IContactNode)
            {
                AddNodeUnknown(node, NodeUnknown.TangentialDisplacement);
                AddNodeUnknown(node, NodeUnknown.LambdaContactDistance);
                AddNodeUnknown(node, NodeUnknown.LambdaContactDirection);
            }
        }

        foreach (var contact in contacts)
        {
            AddContactUnknown(contact, ContactUnknown.RadialDisplacement);
            AddContactUnknown(contact, ContactUnknown.AngleDisplacement);
            AddContactUnknown(contact, ContactUnknown.RotationDisplacement);
        }
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

    private int _index;
    private readonly Dictionary<(Guid, NodeUnknown), int> _nodeUnknownIndices = new();
    private readonly Dictionary<(Guid, Guid, ContactUnknown), int> _contactUnknownIndices = new();

    public int this[GlobalUnknown unknown] => (int)unknown;

    public int this[INode node, NodeUnknown unknown] => _nodeUnknownIndices[(node.Id, unknown)];
    
    public int this[Guid nodeId, NodeUnknown unknown] => _nodeUnknownIndices[(nodeId, unknown)];

    public int this[IParticleContact contact, ContactUnknown unknown] => _contactUnknownIndices[(contact.From.Id, contact.To.Id, unknown)];
    
    public int this[Guid fromId, Guid toId, ContactUnknown unknown] => _contactUnknownIndices[(fromId, toId, unknown)];
}