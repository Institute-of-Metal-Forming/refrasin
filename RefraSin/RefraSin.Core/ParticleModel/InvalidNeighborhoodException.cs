using System;

namespace RefraSin.Core.ParticleModel
{
    public class InvalidNeighborhoodException : Exception
    {
        public InvalidNeighborhoodException(Node sourceNode, Neighbor requestedNeighbor) : this(null, sourceNode, requestedNeighbor) { }

        public InvalidNeighborhoodException(Exception? innerException, Node sourceNode, Neighbor requestedNeighbor) : base(string.Empty,
            innerException)
        {
            SourceNode = sourceNode;
            RequestedNeighbor = requestedNeighbor;
            Message = $"The node {sourceNode.ToString()} has no {requestedNeighbor.ToString().ToLower()} neighbor.";
        }

        public Node SourceNode { get; }

        public Neighbor RequestedNeighbor { get; }

        public override string Message { get; }

        public enum Neighbor
        {
            Upper,
            Lower
        }
    }
}