namespace RefraSin.TEPSolver.TimeIntegration.StepVectors;

public class NodeView
{
    private readonly StepVector _vector;
    private readonly Guid _nodeId;

    public NodeView(StepVector vector, Guid nodeId)
    {
        _vector = vector;
        _nodeId = nodeId;
    }

    public double NormalDisplacement => _vector[_vector.StepVectorMap.GetIndex(_nodeId, NodeUnknown.NormalDisplacement)];
    public double FluxToUpper => _vector[_vector.StepVectorMap.GetIndex(_nodeId, NodeUnknown.FluxToUpper)];
    public double Lambda2 => _vector[_vector.StepVectorMap.GetIndex(_nodeId, NodeUnknown.Lambda2)];
}