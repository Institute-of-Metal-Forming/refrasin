namespace RefraSin.TEPSolver.StepVectors;

public class NodeView
{
    private readonly StepVector _vector;
    private readonly Guid _nodeId;

    public NodeView(StepVector vector, Guid nodeId)
    {
        _vector = vector;
        _nodeId = nodeId;
    }

    public double NormalDisplacement => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.NormalDisplacement]];
    public double FluxToUpper => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.FluxToUpper]];
    public double LambdaVolume => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.LambdaVolume]];
}