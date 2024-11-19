using RefraSin.Numerics.Exceptions;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

/// <summary>
/// An exception that is thrown, if an instability in time integration was detected.
/// </summary>
public class InstabilityException : InvalidStepException
{
    public InstabilityException(
        SolutionState baseState,
        StepVector stepVector,
        Guid particleId,
        Guid nodeId,
        int index
    )
        : base(
            baseState,
            stepVector,
            $"Instability detected above node {nodeId} at index {index} of particle {particleId}."
        )
    {
        NodeId = nodeId;
        ParticleId = particleId;
        Index = index;
    }

    /// <summary>
    /// The GUID of the node where the exception occured.
    /// </summary>
    public Guid NodeId { get; }

    /// <summary>
    /// The GUID of the particle where the exception occured.
    /// </summary>
    public Guid ParticleId { get; }

    /// <summary>
    /// The index of the node in the particle surface.
    /// </summary>
    public int Index { get; }
}
