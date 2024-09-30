using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
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
                if (node is not INodeContact)
                {
                    AddUnknown(node.Id, Unknown.LambdaVolume);
                    AddUnknown(node.Id, Unknown.FluxToUpper);
                    AddUnknown(node.Id, Unknown.NormalDisplacement);
                }
            }

            _particleBlocks[particle.Id] = (startIndex, _index - startIndex);
        }

        BorderStart = _index;

        foreach (var contact in currentState.ParticleContacts)
        {
            AddUnknown(contact.MergedId, Unknown.RadialDisplacement);
            AddUnknown(contact.MergedId, Unknown.AngleDisplacement);
            AddUnknown(contact.MergedId, Unknown.RotationDisplacement);
            AddUnknown(contact.MergedId, Unknown.LambdaContactRotation);

            foreach (var contactNode in contact.FromNodes)
            {
                AddUnknown(contactNode.Id, Unknown.LambdaContactDistance);
                AddUnknown(contactNode.Id, Unknown.LambdaContactDirection);
                LinkUnknown(contactNode.Id, contactNode.ContactedNodeId, Unknown.LambdaContactDistance);
                LinkUnknown(contactNode.Id, contactNode.ContactedNodeId, Unknown.LambdaContactDirection);

                AddUnknown(contactNode.Id, Unknown.LambdaVolume);
                AddUnknown(contactNode.Id, Unknown.FluxToUpper);
                AddUnknown(contactNode.Id, Unknown.NormalDisplacement);

                AddUnknown(contactNode.ContactedNodeId, Unknown.LambdaVolume);
                AddUnknown(contactNode.ContactedNodeId, Unknown.FluxToUpper);
                AddUnknown(contactNode.ContactedNodeId, Unknown.NormalDisplacement);

                if (contactNode.Type == NodeType.Neck)
                {
                    AddUnknown(contactNode.Id, Unknown.TangentialDisplacement);
                    AddUnknown(contactNode.ContactedNodeId, Unknown.TangentialDisplacement);
                }
            }
        }

        AddUnknown(Guid.Empty, Unknown.LambdaDissipation);

        BorderLength = _index - BorderStart;
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

    public (int start, int length) this[IParticle particle] => _particleBlocks[particle.Id];

    public int BorderStart { get; }

    public int BorderLength { get; }
    public int LambdaDissipation() => _indices[(Guid.Empty, Unknown.LambdaDissipation)];

    public int NormalDisplacement(INode node) => _indices[(node.Id, Unknown.NormalDisplacement)];

    public int FluxToUpper(INode node) => _indices[(node.Id, Unknown.FluxToUpper)];

    public int LambdaVolume(INode node) => _indices[(node.Id, Unknown.LambdaVolume)];

    public int TangentialDisplacement(INode node) => _indices[(node.Id, Unknown.TangentialDisplacement)];

    public int LambdaContactDistance(INode node) => _indices[(node.Id, Unknown.LambdaContactDistance)];

    public int LambdaContactDirection(INode node) => _indices[(node.Id, Unknown.LambdaContactDirection)];
    
    public int LambdaContactRotation(ParticleContact contact) => _indices[(contact.MergedId, Unknown.LambdaContactRotation)];

    public int RadialDisplacement(ParticleContact contact) => _indices[(contact.MergedId, Unknown.RadialDisplacement)];

    public int AngleDisplacement(ParticleContact contact) => _indices[(contact.MergedId, Unknown.AngleDisplacement)];

    public int RotationDisplacement(ParticleContact contact) => _indices[(contact.MergedId, Unknown.RotationDisplacement)];

    private enum Unknown
    {
        NormalDisplacement,
        TangentialDisplacement,
        FluxToUpper,
        LambdaVolume,
        LambdaContactDistance,
        LambdaContactDirection,
        LambdaContactRotation,
        RadialDisplacement,
        AngleDisplacement,
        RotationDisplacement,
        LambdaDissipation,
    }
}