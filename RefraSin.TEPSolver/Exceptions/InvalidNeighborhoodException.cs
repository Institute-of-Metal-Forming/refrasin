using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.Exceptions;

public class InvalidNeighborhoodException : InvalidOperationException
{
    public InvalidNeighborhoodException(INode sourceNode, Neighbor requestedNeighbor) : this(null, sourceNode, requestedNeighbor) { }

    public InvalidNeighborhoodException(Exception? innerException, INode sourceNode, Neighbor requestedNeighbor) : base(string.Empty,
        innerException)
    {
        SourceNode = sourceNode;
        RequestedNeighbor = requestedNeighbor;
        Message = $"The node {sourceNode} has no {requestedNeighbor.ToString().ToLower()} neighbor.";
    }

    public INode SourceNode { get; }

    public Neighbor RequestedNeighbor { get; }

    public override string Message { get; }

    public enum Neighbor
    {
        Upper,
        Lower
    }
}