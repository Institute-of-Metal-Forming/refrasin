using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(SolutionState currentState)
    {
        _index = 0;

        foreach (var particle in currentState.Particles)
        {
            foreach (var node in particle.Nodes)
            {
                AddUnknown(node.Id, Unknown.NormalDisplacement);
                if (node is NeckNode)
                    AddUnknown(node.Id, Unknown.TangentialDisplacement);
                AddUnknown(node.Id, Unknown.FluxToUpper);
                AddUnknown(node.Id, Unknown.LambdaVolume);

                if (node is ContactNodeBase contactNode)
                {
                    AddUnknown(contactNode.Id, Unknown.LambdaContactX);
                    AddUnknown(contactNode.Id, Unknown.LambdaContactY);
                    LinkUnknown(
                        contactNode.Id,
                        contactNode.ContactedNodeId,
                        Unknown.LambdaContactX
                    );
                    LinkUnknown(
                        contactNode.Id,
                        contactNode.ContactedNodeId,
                        Unknown.LambdaContactY
                    );
                }
            }

            AddUnknown(particle.Id, Unknown.ParticleDisplacementX);
            AddUnknown(particle.Id, Unknown.ParticleDisplacementY);
        }

        AddUnknown(Guid.Empty, Unknown.LambdaDissipation);

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

    public int LambdaDissipation() => _indices[(Guid.Empty, Unknown.LambdaDissipation)];

    public int NormalDisplacement(INode node) => _indices[(node.Id, Unknown.NormalDisplacement)];

    public int FluxToUpper(INode node) => _indices[(node.Id, Unknown.FluxToUpper)];

    public int LambdaVolume(INode node) => _indices[(node.Id, Unknown.LambdaVolume)];

    public int TangentialDisplacement(INode node) =>
        _indices[(node.Id, Unknown.TangentialDisplacement)];

    public int LambdaContactX(INode node) => _indices[(node.Id, Unknown.LambdaContactX)];

    public int LambdaContactY(INode node) => _indices[(node.Id, Unknown.LambdaContactY)];

    public int ParticleDisplacementX(Particle particle) =>
        _indices[(particle.Id, Unknown.ParticleDisplacementX)];

    public int ParticleDisplacementY(Particle particle) =>
        _indices[(particle.Id, Unknown.ParticleDisplacementY)];

    private enum Unknown
    {
        NormalDisplacement,
        TangentialDisplacement,
        FluxToUpper,
        LambdaVolume,
        LambdaContactX,
        LambdaContactY,
        ParticleDisplacementX,
        ParticleDisplacementY,
        LambdaDissipation,
    }
}
