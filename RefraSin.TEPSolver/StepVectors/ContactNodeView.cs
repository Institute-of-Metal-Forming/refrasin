namespace RefraSin.TEPSolver.StepVectors;

public class ContactNodeView
{
    private readonly StepVector _vector;
    private readonly Guid _nodeId;

    public ContactNodeView(StepVector vector, Guid nodeId)
    {
        _vector = vector;
        _nodeId = nodeId;
    }

    public double NormalDisplacement => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.NormalDisplacement]];
    public double FluxToUpper => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.FluxToUpper]];
    public double LambdaVolume => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.LambdaVolume]];
    
    public double TangentialDisplacement => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.TangentialDisplacement]];
    public double LambdaContactDistance => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.LambdaContactDistance]];
    public double LambdaContactDirection => _vector[_vector.StepVectorMap[_nodeId, NodeUnknown.LambdaContactDirection]];
}