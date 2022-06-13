using System;
using RefraSin.Core.ParticleModel;

namespace RefraSin.Core.Solver
{
    /// <summary>
    /// Exception raised when the time step calculation failed.
    /// </summary>
    public class InvalidTimeStepException : Exception
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="sourceNode">the node where the time step was invalid</param>
        /// <param name="reason">the reason of invalidity</param>
        public InvalidTimeStepException(Node sourceNode, InvalidityReason reason = InvalidityReason.Unspecified)
        {
            SourceNode = sourceNode;
            Reason = reason;
            Message = $"Time step of node {sourceNode} was invalid due to: {reason}";
        }

        /// <summary>
        /// The node where the time step was invalid.
        /// </summary>
        public Node SourceNode { get; }

        /// <summary>
        /// The reason why the time step failed.
        /// </summary>
        public InvalidityReason Reason { get; }

        /// <inheritdoc />
        public override string Message { get; }
    }

    /// <summary>
    /// Reason why the time step failed.
    /// </summary>
    public enum InvalidityReason
    {
        /// <summary>
        /// No further information.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Distance of movement was too large.
        /// </summary>
        MovementToLarge,
        
        /// <summary>
        /// A numeric algorithm did not converge.
        /// </summary>
        NotConverged
    }
}